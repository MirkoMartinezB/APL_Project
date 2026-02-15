using SupabaseApiCall.Models;

namespace SupabaseApiCall.Repositories
{
    public interface IArticoloRepository
    {
        Task<IReadOnlyList<Articolo>> GetArticoliAsync();
        Task<IReadOnlyList<Articolo>> GetArticoliAsync(HashSet<string> setCodiceArticoli);
        Task<Articolo?> GetArticoloAsync(string codiceArticolo);
    }
}
