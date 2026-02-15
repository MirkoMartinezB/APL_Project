using SupabaseApiCall.Models;



namespace SupabaseApiCall.Repositories

{
    public class ClienteRepository : IClienteRepository
    {
        //
        private readonly Supabase.Client _supabase;

        public ClienteRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IReadOnlyList<Cliente>> GetClientiAsync()
        {
            var response = await _supabase.From<Cliente>().Get();
            return response.Models;
        }
        public async Task<Cliente?> GetClienteAsync(long codiceCliente)
        {
            var response = await _supabase
                .From<Cliente>()
                .Where(c => c.CodiceCliente == codiceCliente)
                .Get();
            return response.Models.FirstOrDefault();
        }

    }
}
