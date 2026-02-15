using SupabaseApiCall.Models;
using static Supabase.Postgrest.Constants;

namespace SupabaseApiCall.Repositories
{
    public class FatturaFornitoreRepository : IFatturaFornitoreRepository
    {
        private readonly Supabase.Client _supabase;

        public FatturaFornitoreRepository(Supabase.Client supabase)
        {
            _supabase = supabase;
        }
        /* Il metodo deve restituire una lista di fatture fornitore che contengono articoli specifici in un intervallo di date.
         * Verrà poi utilizzato per calcolare i prezzi medi di acquisto degli articoli venduti ai clienti.
         * Sfruttando sempre il metodo C++ chiamato tramite P/Invoke. 
         */
        public async Task<IReadOnlyList<FatturaFornitore>> GetFattureAsync(HashSet<string> setCodiceArticoli, DateTime periodoIniziale, DateTime periodoFinale)
        {
            //Where supporta confronti semplici (==, !=, >, <, >=, <=) e combinazioni logiche (AND, OR).
            //Mentre Filter supporta operatori più avanzati come IN, NOT IN, LIKE, ILIKE, IS NULL, IS NOT NULL.
            var response = await _supabase.From<FatturaFornitore>() 
                .Where(f => f.DataFattura >= periodoIniziale)
                .Where(f => f.DataFattura <= periodoFinale)
                .Filter(f => f.CodiceArticolo, Operator.In, setCodiceArticoli.ToArray())
                .Get();
            return response.Models;
        }
    }
}
