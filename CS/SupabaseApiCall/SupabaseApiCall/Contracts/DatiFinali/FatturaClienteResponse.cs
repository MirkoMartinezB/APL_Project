using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiFinali
{
    /* Classe per le fatture Clienti
     * Viene popolata a partire dai dati memorizzati nel database Supabase.
     */

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
