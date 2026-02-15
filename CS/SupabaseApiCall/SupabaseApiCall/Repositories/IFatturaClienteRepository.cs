using SupabaseApiCall.Models;

namespace SupabaseApiCall.Repositories
{
    public interface IFatturaClienteRepository
    {
        Task<IReadOnlyList<FatturaCliente>> GetFattureAsync(long codiceCliente, DateTime periodoIniziale, DateTime periodoFinale);
    }
}
