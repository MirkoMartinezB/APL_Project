using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiFinali
{
    public class FatturaFornitoreResponse : FatturaResponse
    {
        public FatturaFornitoreResponse(FornitoreResponse Fornitore, DateTime DataFattura, long NumeroFattura)
        {

            this.CliFor = Fornitore;
            this.DataFattura = DataFattura;
            this.NumeroFattura = NumeroFattura;
            this.TotaleFattura = 0.0;
            this.TotaleImporto = 0.0;
            this.CorpoFattura = new List<FatturaCorpo>();
        }

    }
}
