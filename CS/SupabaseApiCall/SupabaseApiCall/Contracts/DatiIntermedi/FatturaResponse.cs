using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiIntermedi
{
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
    }



}
