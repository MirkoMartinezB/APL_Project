using SupabaseApiCall.Models;

namespace SupabaseApiCall.Repositories
{
    public class FatturaClienteRepository : IFatturaClienteRepository
    {
        private readonly Supabase.Client _supabase;

        public FatturaClienteRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IReadOnlyList<FatturaCliente>> GetFattureAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale)
        {
            var response = await _supabase.From<FatturaCliente>()
                .Where(f => f.CodiceCliente == codiceCliente)
                .Where(f => f.DataFattura >= periodoIniziale)
                .Where(f => f.DataFattura <= periodoFinale)
                .Get();
            return response.Models;
        }
    }
}
