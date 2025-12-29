using SupabaseApiCall.Contracts.Anagrafiche;
using SupabaseApiCall.Contracts.DatiIntermedi;

namespace SupabaseApiCall.Contracts.CruscottiFinali
{
    public class CruscottoClienteResponse
    {

        public ClienteResponse Cliente { get; set; }

        public Dictionary<string, ArticoloResponse> ArticoliMovimentati { get; set; }

        //gestione archivio ricevute
        public List<FatturaClienteResponse> FattureCliente { get; set; }

        

        private CruscottoClienteResponse(CruscottoClienteResponseBuilder builder)
        {           
            this.Cliente = builder.Cliente;
            this.ArticoliMovimentati = builder.ArticoliMovimentati;
            this.FattureCliente = builder.FattureCliente;
        }

        public class CruscottoClienteResponseBuilder
        {

            public ClienteResponse Cliente { get; set; }

            public Dictionary<string, ArticoloResponse> ArticoliMovimentati { get; set; }

            //gestione archivio ricevute
            public List<FatturaClienteResponse> FattureCliente { get; set; }


            public CruscottoClienteResponseBuilder setCliente(ClienteResponse cliente)
            {
                this.Cliente = cliente;
                return this;
            }

            public CruscottoClienteResponseBuilder setArticoliMovimentati(Dictionary<string, ArticoloResponse> ArticoliMovimentati)
            {
                this.ArticoliMovimentati = ArticoliMovimentati;
                return this;
            }

            public CruscottoClienteResponseBuilder setFattureCliente(List<FatturaClienteResponse> FattureCliente)
            {
                this.FattureCliente = FattureCliente;
                return this;
            }

            public CruscottoClienteResponse Build()
            {
                return new CruscottoClienteResponse(this);
            }

        }

    }   

}
