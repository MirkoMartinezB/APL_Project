using SupabaseApiCall.Models;

namespace SupabaseApiCall.Repositories
{
    public interface IFatturaFornitoreRepository
    {
        // Il metodo deve restituire una lista di fatture fornitore che contengono articoli specifici in un intervallo di date.
        
        Task<IReadOnlyList<FatturaFornitore>> GetFattureAsync(HashSet<string> setCodiceArticoli, DateTime periodoIniziale, DateTime periodoFinale);
    }
}
