using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiIntermedi
{
    /* La classe StatMensiliMedieResponse rappresenta la risposta per le statistiche mensili dei prezzi medi e delle quantità movimentate di un articolo specifico.
     * Viene costruita a partire dai dati delle fatture elaborate e calcolate tramite un servizio esterno (PrezziMediArticoliExternalService).
     * La classe C++ non restituisce direttamente questa struttura, ma fornisce i dati necessari per popolarla in C#.
     * Quindi la classe C++ deve essere incapsulata in modo che i dati restituiti possano essere mappati correttamente in questa struttura C#.
     */
    public class StatMensiliMedieResponse
    {
        public ArticoloResponse Articolo { get; set; }  
        public Dictionary<string, double> PrezzoMedioVenditaMese { get; set; } //Prezzo medio di Vendita per anno/mese
        public Dictionary<string, double> QuantitàMovimentataVenditaMese { get; set; } //Totale movimentato in vendita per anno/mese
        public Dictionary<string, double> PrezzoMedioAcquistoMese { get; set; } //Prezzo medio di Acquisto per anno/mese
        public Dictionary<string, double> QuantitàMovimentataAcquistoMese { get; set; } //Totale movimentato in acquisto per anno/mese
        public double PrezzoMedioVenditaStatisticaK { get; set; } //Prezzo medio di vendita considerando gli ultimi K mesi presenti nel dizionario
        public double PrezzoMedioAcquistoStatisticaK { get; set; } //Prezzo medio di acquisto considerando gli ultimi K mesi presenti nel dizionario

        public string buildKeyAnnoMese(int anno, int mese)
        {
            //return anno.ToString() + "_" + mese.ToString();
            return $"{anno}_{mese:00}"; 
        }

        public void buildStatsK(int k = 3)
        {
            PrezzoMedioVenditaStatisticaK = calculateStatisticaKfromDicts(PrezzoMedioVenditaMese, QuantitàMovimentataVenditaMese, k);
            PrezzoMedioAcquistoStatisticaK = calculateStatisticaKfromDicts(PrezzoMedioAcquistoMese, QuantitàMovimentataAcquistoMese, k);
        }

        //Calcola il prezzo medio di vendita considerando gli ultimi K mesi presenti
        //Dato che la classe aggrega sia prezzo medio di acquisto che di vendita, 
        //Può esserci un mese in cui è presente solo uno dei due (solo acquisto o solo vendita), quindi la quantità movimentata di quel tipo è 0 e sarà da scartare come mese. 
        //Quindi la statistica k-esima considera solo gli ultimi mesi in cui la qtaMovimentata è presente. 
        //Chiaramente se il numero di mesi con qtaMovimentata presente è inferiore a k, allora la statistica considera solo i mesi presenti nel calcolo.
        private double calculateStatisticaKfromDicts(Dictionary<string, double> PrezzoMedioMese, Dictionary<string, double> QuantitàMovimentataMese, int k = 3)
        {
            //Utilizzo LinQ per estrapolare gli ultimi k-elementi che hanno qta movimentata, estrapolando quindi la key
            var keysDesc = QuantitàMovimentataMese
                .Where(kv => kv.Value > 0)
                .Select(kv => kv.Key)
                .OrderByDescending(k => k)
                .Take(k)
                .ToList();
                
            //Utilizzo le key per verificare i mesi effettivi e restituire la statistica
            int numMonthsConsidered = keysDesc.Count();
            if (numMonthsConsidered == 0)
                return 0.0;

            double qtaMovimentata = 0.0;
            double value = 0.0;
            foreach(var key in keysDesc)
            {
                if (QuantitàMovimentataMese[key] > 0)
                {
                    qtaMovimentata += QuantitàMovimentataMese[key];
                    value += QuantitàMovimentataMese[key] * PrezzoMedioMese[key]; 
                }
            }

            return Math.Round(value / qtaMovimentata, 2); 
        }

    }

    
}
