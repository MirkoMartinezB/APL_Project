using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Services;

public interface IClienteService
{
    /* L'implementazione di questo servizio deve essere in grado di:
     * - Recuperare tutti i clienti
     * - Recuperare un singolo cliente dato il codice cliente
     */
    Task<IReadOnlyList<ClienteResponse>> GetClientiAsync();
    Task<ClienteResponse?> GetClienteAsync(long codiceCliente);
}