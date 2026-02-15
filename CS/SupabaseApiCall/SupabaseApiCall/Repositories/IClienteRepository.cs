using SupabaseApiCall.Models;

namespace SupabaseApiCall.Repositories;
// Interfaccia che rappresenta il contratto per l'accesso ai dati dei Clienti.
// Consente di:
// - separare la responsabilità di accesso ai dati dalla logica applicativa
// - disaccoppiare il service dalla tecnologia di persistenza (Supabase, DB, API, ecc.)
// - facilitare test unitari e future estensioni

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> GetClientiAsync();
    Task<Cliente?> GetClienteAsync(long codiceCliente);
}
