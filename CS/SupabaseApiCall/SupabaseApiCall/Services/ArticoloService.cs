using SupabaseApiCall.Contracts.Anagrafiche;
using SupabaseApiCall.Repositories;

namespace SupabaseApiCall.Services
{
    //Per commenti vedi ClienteService
    public class ArticoloService : IArticoloService
    {
        private readonly IArticoloRepository _repo;
        public ArticoloService(IArticoloRepository repo)
        {
            _repo = repo;
        }
        
        public async Task<IReadOnlyList<ArticoloResponse>> GetArticoliAsync()
        {
            var articoli = await _repo.GetArticoliAsync();

            return articoli
                .Select(a => new ArticoloResponse
                {
                    CodiceArticolo = a.CodiceArticolo,
                    DescrizioneArticolo = a.DescrizioneArticolo
                })
                .ToList();
        }
        public async Task<IReadOnlyList<ArticoloResponse>> GetArticoliAsync(HashSet<string> setCodiceArticoli)
        {
            var articoli = await _repo.GetArticoliAsync(setCodiceArticoli);

            return articoli
                .Select(a => new ArticoloResponse
                {
                    CodiceArticolo = a.CodiceArticolo,
                    DescrizioneArticolo = a.DescrizioneArticolo
                })
                .ToList();
        }

        public async Task<ArticoloResponse?> GetArticoloAsync(string codiceArticolo)
        {
            var articolo = await _repo.GetArticoloAsync(codiceArticolo);


            if (articolo is null)
            {
                return null;
            }

            var articoloResponse = new ArticoloResponse
            {
                CodiceArticolo = articolo.CodiceArticolo,
                DescrizioneArticolo = articolo.DescrizioneArticolo
            };

            return articoloResponse;
        }

    }
}
