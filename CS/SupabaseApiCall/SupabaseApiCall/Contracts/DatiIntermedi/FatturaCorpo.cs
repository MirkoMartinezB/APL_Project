using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiIntermedi
{
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
