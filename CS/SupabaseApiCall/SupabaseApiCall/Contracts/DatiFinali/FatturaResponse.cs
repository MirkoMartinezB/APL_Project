using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiFinali
{
    /*  Classe base per le fatture Clienti e Fornitori
     *  Contiene le informazioni sia della testata che del corpo della fattura comuni a entrambe le tipologie.
     *  Nella costruizone dell'oggetto bisogna gestire benee la controparte (CliFor) in modo che sia del tipo corretto.
     *  Tipicamente l'oggetto restituito è una lista (FatturaTypeResponse) che contiene la testata del documento,
     *  con un'ulteriore lista incpsulata (CorpoFattura) che ne detiene le righe.
     */
    public abstract class FatturaResponse
    {

        public Controparte CliFor { get; set; }

        public DateTime DataFattura { get; set; }

        public long NumeroFattura { get; set; }

        public List<FatturaCorpo> CorpoFattura { get; set; }

        public double TotaleFattura { get; set; }

        public double TotaleImporto { get; set; }





        public void AggiungiCorpo(ArticoloResponse articolo, double qtaMovimento, double prezzo, double scontoPerc, double importo, double importoIvato, double importoIva, int aliquotaIva)
        {
            var corpo = new FatturaCorpo
            {
                Articolo = articolo,
                QtaMovimento = qtaMovimento,
                Prezzo = prezzo,
                ScontoPerc = scontoPerc,
                Importo = importo,
                ImportoIvato = importoIvato,
                ImportoIva = importoIva,
                AliquotaIva = aliquotaIva
            };
            this.CorpoFattura.Add(corpo);
        }

        public void ricalcolaTotali()
        {
            TotaleFattura = CorpoFattura.Sum(c => c.ImportoIvato);
            TotaleImporto = CorpoFattura.Sum(c => c.Importo);
        }

    }

}