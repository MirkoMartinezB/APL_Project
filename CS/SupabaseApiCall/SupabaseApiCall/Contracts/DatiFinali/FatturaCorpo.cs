using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiFinali
{
    /*  Classe per il corpo della fattura
     *  Contiene le informazioni relative alle singole righe della fattura.
     *  Tipicamente una FatturaCorpo non può esistere senza essere parte di una FatturaTypeResponse.
     */
    public class FatturaCorpo
    {
        public ArticoloResponse Articolo { get; set; }

        public double QtaMovimento { get; set; }

        public double Prezzo { get; set; }

        public double ScontoPerc { get; set; }

        public double Importo { get; set; }

        public double ImportoIvato { get; set; }

        public double ImportoIva { get; set; }

        public int AliquotaIva { get; set; }
    }
}
