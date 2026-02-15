using SupabaseApiCall.Models;
using static Supabase.Postgrest.Constants;



namespace SupabaseApiCall.Repositories
{
    public class ArticoloRepository : IArticoloRepository
    {

        private readonly Supabase.Client _supabase;

        public ArticoloRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IReadOnlyList<Articolo>> GetArticoliAsync()
        {
            var response = await _supabase.From<Articolo>().Get();
            return response.Models;
        }
        public async Task<IReadOnlyList<Articolo>> GetArticoliAsync(HashSet<string> setCodiceArticoli)
        {
            
            var response = await _supabase
                .From<Articolo>()
                .Filter(a => a.CodiceArticolo, Operator.In, setCodiceArticoli.ToArray())
                .Get();

            return response.Models;
        }
        public async Task<Articolo?> GetArticoloAsync(string codiceArticolo)
        {
            var response = await _supabase
                .From<Articolo>()
                .Where(a => a.CodiceArticolo == codiceArticolo)
                .Get();
            return response.Models.FirstOrDefault();
        }
    }
}
