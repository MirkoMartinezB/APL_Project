using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Services
{
    public interface IArticoloService
    {
        /* L'implementazione di questo servizio deve essere in grado di:
         * - Recuperare tutti gli articoli
         * - Recuperare gli articoli dato un insieme di codici articolo
         * - Recuperare un singolo articolo dato il codice articolo
         */
        Task<IReadOnlyList<ArticoloResponse>> GetArticoliAsync();
        Task<IReadOnlyList<ArticoloResponse>> GetArticoliAsync(HashSet<string> setCodiceArticoli);
        Task<ArticoloResponse?> GetArticoloAsync(string codiceArticolo);
    }
}
