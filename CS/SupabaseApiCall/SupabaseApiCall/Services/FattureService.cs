using SupabaseApiCall.Contracts.Anagrafiche;
using SupabaseApiCall.Contracts.DatiFinali;
using SupabaseApiCall.Contracts.DatiIntermedi;
using SupabaseApiCall.Repositories;
using SupabaseApiCall.ServicesExternal;
using static System.Net.Mime.MediaTypeNames;


namespace SupabaseApiCall.Services
{
    public class FattureService : IFattureService
    {
        private readonly IFatturaClienteRepository _fattureRepo;
        private readonly IFatturaFornitoreRepository _fattureFornitoreRepo;
        private readonly IClienteService _clienteService;
        private readonly IArticoloService _articoloService;


        public FattureService(IFatturaClienteRepository fattureRepo,
            IFatturaFornitoreRepository fattureFornitoreRepo,
            IClienteService clienteService, 
            IArticoloService articoloService)  
        {
            _fattureRepo = fattureRepo;
            _clienteService = clienteService;
            _articoloService = articoloService;
            _fattureFornitoreRepo = fattureFornitoreRepo;
        }
        
        public async Task<IReadOnlyList<FatturatoMensileResponse>> GetFatturatoMensileAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale)
        {
            
            var fatture = await _fattureRepo.GetFattureAsync(codiceCliente, periodoIniziale, periodoFinale);            
            

            var fatturatoMensileList = fatture
                .GroupBy(f => new { f.DataFattura.Year, f.DataFattura.Month })
                .Select(g => new FatturatoMensileResponse
                {
                    Anno = g.Key.Year,
                    Mese = g.Key.Month,
                    Fatturato = g.Sum(f => f.ImportoIvato)
                })
                .OrderBy(r => r.Anno)
                .ThenBy(r => r.Mese)
                .ToList();

            return fatturatoMensileList;
        }
        public async Task<IReadOnlyList<FatturaResponse>> GetListaFattureAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale)
        {
            
            var clienteResponse = await _clienteService.GetClienteAsync(codiceCliente);
            // Dict di articoli per evitare chiamate ripetute interne al ciclo
            var articoliDict = _articoloService.GetArticoliAsync().Result.ToDictionary(a => a.CodiceArticolo ?? string.Empty, a => a);

            var dicFatture = new Dictionary<(int anno, long numero), FatturaClienteResponse>(); // ricostruisco le fatture con il loro dettaglio

            var fatture = await _fattureRepo.GetFattureAsync(codiceCliente, periodoIniziale, periodoFinale);
            var listaFattureResponse = new List<FatturaClienteResponse>();  
            foreach (var fattura in fatture)
            {
                var key = (fattura.DataFattura.Year, fattura.NumeroFattura);

                if (!dicFatture.TryGetValue(key, out var fatturaClienteResponse))
                {
                    fatturaClienteResponse = new FatturaClienteResponse(
                        clienteResponse,
                        fattura.DataFattura,
                        fattura.NumeroFattura
                    );

                    dicFatture.Add(key, fatturaClienteResponse);
                    listaFattureResponse.Add(fatturaClienteResponse);
                }

                if (!articoliDict.ContainsKey(fattura.CodiceArticolo))
                {
                    var articolo = new ArticoloResponse
                    {
                        CodiceArticolo = fattura.CodiceArticolo,
                        DescrizioneArticolo = "XX-PRODOTTO NON TROVATO-XX"
                    };
                    articoliDict.Add(fattura.CodiceArticolo, articolo);                    
                }


                fatturaClienteResponse.TotaleFattura += fattura.ImportoIvato;
                fatturaClienteResponse.TotaleImporto += fattura.Importo;
                fatturaClienteResponse.AggiungiCorpo(
                    articoliDict[fattura.CodiceArticolo],
                    fattura.QtaMovimento,
                    fattura.Prezzo,
                    fattura.ScontoPerc,
                    fattura.Importo,
                    fattura.ImportoIvato,
                    fattura.ImportoIva,
                    fattura.AliquotaIva
                );
                
            }
            return listaFattureResponse;
        }
        /*Questo rappresenta il metodo più complesso del servizio.
         * Deve recuperare le fatture emesse a un cliente in un intervallo di date specificato, 
         * calcolare le statistiche mensili medie per ciascun articolo venduto, 
         * tenendo conto anche dei movimenti fornitore relativi agli articoli venduti al cliente.
         * Utilizza un servizio esterno C++ per il calcolo delle medie. 
         * Infine combina i dati delle fatture clienti e fornitori per costruire la risposta finale.
         */
        public async Task<IReadOnlyList<StatMensiliMedieResponse>> GetStatisticheFattureAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale, int k)
        {
            var dicStatMensiliMedieResponse = new Dictionary<ArticoloResponse, StatMensiliMedieResponse>();

            var fatture = await _fattureRepo.GetFattureAsync(codiceCliente, periodoIniziale, periodoFinale);
            var arrayFatturaParseCpp = fatture
                .Select(f => new FatturaParseCpp
                {
                    CodiceArticolo = f.CodiceArticolo,
                    Mese = f.DataFattura.Month,
                    Anno = f.DataFattura.Year,
                    QtaMovimento = f.QtaMovimento,
                    Importo = f.Importo                    
                })
                .ToArray();
            var arrayFattureResponseTmpCpp = new FatturaParseCpp[arrayFatturaParseCpp.Length]; //array di risposta, dimensione massima come quella precendente ma sarà modificata

            nuint returnedSize = 0;
            // Chiamata al servizio esterno C++ per il calcolo delle medie
            PrezziMediArticoliExternalService.AggregateInvoicesByProductYearMonth(arrayFatturaParseCpp, (nuint)arrayFatturaParseCpp.Length, arrayFattureResponseTmpCpp, out returnedSize);

            int sizeFinale = checked((int)returnedSize);
            var setArticoliMovimentati = new HashSet<string>(arrayFattureResponseTmpCpp.Take(sizeFinale).Select(f => f.CodiceArticolo));
            // Recupero i dati degli articoli movimentati
            var dicArticoliMovimentati = (await _articoloService.GetArticoliAsync())
                .Where(a => setArticoliMovimentati.Contains(a.CodiceArticolo ?? string.Empty))
                .ToDictionary(a => a.CodiceArticolo ?? string.Empty, a => a);

            // Costruisco un dizionario per ricostruire la risposta in maniera efficiente
            foreach (var fatturaParseCpp in arrayFattureResponseTmpCpp.Take(sizeFinale))
            {
                if (!dicArticoliMovimentati.ContainsKey(fatturaParseCpp.CodiceArticolo))
                {
                    var articoloNonTrovato = new ArticoloResponse
                    {
                        CodiceArticolo = fatturaParseCpp.CodiceArticolo,
                        DescrizioneArticolo = "XX-PRODOTTO NON TROVATO-XX"
                    };
                    dicArticoliMovimentati.Add(fatturaParseCpp.CodiceArticolo, articoloNonTrovato);
                }
                var articoloResponse = dicArticoliMovimentati[fatturaParseCpp.CodiceArticolo];
                if (!dicStatMensiliMedieResponse.TryGetValue(articoloResponse, out var statMensiliMedieResponse))
                {
                    statMensiliMedieResponse = new StatMensiliMedieResponse
                    {
                        Articolo = articoloResponse,
                        PrezzoMedioVenditaMese = new Dictionary<string, double>(),
                        PrezzoMedioAcquistoMese = new Dictionary<string, double>(),
                        QuantitàMovimentataVenditaMese = new Dictionary<string, double>(),
                        QuantitàMovimentataAcquistoMese = new Dictionary<string, double>()
                    };
                    dicStatMensiliMedieResponse.Add(articoloResponse, statMensiliMedieResponse);
                }
                var key = statMensiliMedieResponse.buildKeyAnnoMese(fatturaParseCpp.Anno, fatturaParseCpp.Mese);
                statMensiliMedieResponse.QuantitàMovimentataVenditaMese[key] = fatturaParseCpp.QtaMovimento;
                statMensiliMedieResponse.PrezzoMedioVenditaMese[key] = fatturaParseCpp.QtaMovimento != 0 ? fatturaParseCpp.Importo / fatturaParseCpp.QtaMovimento : 0;
                statMensiliMedieResponse.PrezzoMedioVenditaMese[key] = Math.Round(statMensiliMedieResponse.PrezzoMedioVenditaMese[key], 2);
                statMensiliMedieResponse.PrezzoMedioAcquistoMese[key] = 0; // valorizzato successivamente
                statMensiliMedieResponse.QuantitàMovimentataAcquistoMese[key] = 0;
            }
            //Stesse operazioni di sopra ripetute per le fatture fornitore
            var fattureFornitori = await _fattureFornitoreRepo.GetFattureAsync(setArticoliMovimentati, periodoIniziale, periodoFinale);
            arrayFatturaParseCpp = fattureFornitori
                .Select(f => new FatturaParseCpp
                {
                    CodiceArticolo = f.CodiceArticolo,
                    Mese = f.DataFattura.Month,
                    Anno = f.DataFattura.Year,
                    QtaMovimento = f.QtaMovimento,
                    Importo = f.Importo
                })
                .ToArray();
            arrayFattureResponseTmpCpp = new FatturaParseCpp[arrayFatturaParseCpp.Length]; //array di risposta, dimensione massima come quella precendente ma sarà modificata
            returnedSize = 0;
            // Chiamata al servizio esterno C++ per il calcolo delle medie
            PrezziMediArticoliExternalService.AggregateInvoicesByProductYearMonth(arrayFatturaParseCpp, (nuint)arrayFatturaParseCpp.Length, arrayFattureResponseTmpCpp, out returnedSize);
            sizeFinale = checked((int)returnedSize);
            foreach (var fatturaParseCpp in arrayFattureResponseTmpCpp.Take(sizeFinale))
            {
                /* Dato i filtri di sopra questo controllo non dovrebbe mai essere vero,
                 * ma lo lascio per sicurezza nel caso in futuro si modifichino i filtri di ricerca
                 */
                if (!dicArticoliMovimentati.ContainsKey(fatturaParseCpp.CodiceArticolo))
                {
                    var articoloNonTrovato = new ArticoloResponse
                    {
                        CodiceArticolo = fatturaParseCpp.CodiceArticolo,
                        DescrizioneArticolo = "XX-PRODOTTO NON TROVATO-XX"
                    };
                    dicArticoliMovimentati.Add(fatturaParseCpp.CodiceArticolo, articoloNonTrovato);
                }
                var articoloResponse = dicArticoliMovimentati[fatturaParseCpp.CodiceArticolo];
                if (!dicStatMensiliMedieResponse.TryGetValue(articoloResponse, out var statMensiliMedieResponse))
                {
                    statMensiliMedieResponse = new StatMensiliMedieResponse
                    {
                        Articolo = articoloResponse,
                        PrezzoMedioVenditaMese = new Dictionary<string, double>(),
                        PrezzoMedioAcquistoMese = new Dictionary<string, double>(),
                        QuantitàMovimentataVenditaMese = new Dictionary<string, double>(),
                        QuantitàMovimentataAcquistoMese = new Dictionary<string, double>()
                    };
                    dicStatMensiliMedieResponse.Add(articoloResponse, statMensiliMedieResponse);
                }
                var key = statMensiliMedieResponse.buildKeyAnnoMese(fatturaParseCpp.Anno, fatturaParseCpp.Mese);
                statMensiliMedieResponse.QuantitàMovimentataAcquistoMese[key] = fatturaParseCpp.QtaMovimento;
                statMensiliMedieResponse.PrezzoMedioAcquistoMese[key] = fatturaParseCpp.QtaMovimento != 0 ? fatturaParseCpp.Importo / fatturaParseCpp.QtaMovimento : 0; // Non gestito nelle fatture clienti
                statMensiliMedieResponse.PrezzoMedioAcquistoMese[key] = Math.Round(statMensiliMedieResponse.PrezzoMedioAcquistoMese[key], 2);
            }



            var listaStatMensiliMedieResponse = dicStatMensiliMedieResponse.Values.ToList();
            foreach(var statMensiliMedieResponse in listaStatMensiliMedieResponse)
            {
                statMensiliMedieResponse.buildStatsK(k);
            }



            return listaStatMensiliMedieResponse;
        }

    }
}
