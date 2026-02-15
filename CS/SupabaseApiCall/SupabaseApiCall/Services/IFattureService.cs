using SupabaseApiCall.Contracts.DatiFinali;
using SupabaseApiCall.Contracts.DatiIntermedi;
using SupabaseApiCall.Contracts.Anagrafiche;
namespace SupabaseApiCall.Services
{
    public interface IFattureService
    {
        /* Il metodo deve restituire il fatturato mensile di un cliente in un intervallo di date specificato.
         * Il risultato è una lista di oggetti FatturatoMensileResponse, ciascuno rappresentante il fatturato per un mese specifico.
         */
        Task<IReadOnlyList<FatturatoMensileResponse>> GetFatturatoMensileAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale);
        /* Il metodo deve restituire una lista di fatture emesse a un cliente in un intervallo di date specificato.
         * Il risultato è una lista di oggetti FatturaClienteResponse, ciascuno rappresentante una fattura.
         */
        Task<IReadOnlyList<FatturaResponse>> GetListaFattureAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale);
        
        /* Il metodo deve restituire le statistiche mensili medie per ciascun articolo venduto a un cliente in un intervallo di date specificato.
         * Il risultato è un dizionario in cui la chiave è un oggetto ArticoloResponse e il valore è un oggetto StatMensiliMedieResponse.
         * Il metodo deve tenere in considerazione anche i movimenti fornitore relativi agli articoli venduti al cliente in modo da calcolare 
         * correttamente i prezzi medi di acquisto.
         */
        Task<IReadOnlyList<StatMensiliMedieResponse>> GetStatisticheFattureAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale, int k);

    }
}
