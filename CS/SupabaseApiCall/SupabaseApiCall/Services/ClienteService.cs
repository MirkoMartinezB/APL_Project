using SupabaseApiCall.Contracts.Anagrafiche;
using SupabaseApiCall.Models;
using SupabaseApiCall.Repositories;

namespace SupabaseApiCall.Services;
// Service che contiene la logica applicativa relativa ai Clienti
// Si occupa di: 
// - orchestrare l'accesso ai dati tramite il repository
// - mappa il modello (Cliente) nei DTO esposti dall'API (ClienteResponse)
public class ClienteService : IClienteService
{
    // Repository per l'accesso ai dati dei Clienti,
    // iniettato tramite DI in modo da disaccoppiare il srevizio. 
    private readonly IClienteRepository _repo;
    public ClienteService(IClienteRepository repo)
    {
        _repo = repo;
    }
    // Recupera l'elenco dei Clienti
    public async Task<IReadOnlyList<ClienteResponse>> GetClientiAsync()
    {
        var clienti = await _repo.GetClientiAsync();

        return clienti
            .Select(c => new ClienteResponse
            {
                Codice = c.CodiceCliente,
                RagioneSociale = c.RagioneSociale
            })
            .ToList();
    }
    public async Task<ClienteResponse?> GetClienteAsync(long codiceCliente)
    {
        var cliente = await _repo.GetClienteAsync(codiceCliente);


        if (cliente is null)
        {
            return null;
        }

        var clienteResponse = new ClienteResponse
        {
            Codice = cliente.CodiceCliente,
            RagioneSociale = cliente.RagioneSociale
        };

        return clienteResponse;
    }
}
