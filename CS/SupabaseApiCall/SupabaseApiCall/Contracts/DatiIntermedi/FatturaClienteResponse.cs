using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiIntermedi
{
    public class FatturaClienteResponse : FatturaResponse
    {
        public FatturaClienteResponse(ClienteResponse Cliente, DateTime DataFattura, long NumeroFattura)
        {

            this.CliFor = Cliente;
            this.DataFattura = DataFattura;
            this.NumeroFattura = NumeroFattura;
            this.TotaleFattura = 0.0;
            this.TotaleImporto = 0.0;
            this.CorpoFattura = new List<FatturaCorpo>();
        }
    }
}
