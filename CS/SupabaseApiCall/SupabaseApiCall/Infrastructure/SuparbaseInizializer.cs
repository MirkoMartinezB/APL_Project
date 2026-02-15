namespace SupabaseApiCall.Infrastructure
{
    /*
     * Questa classe serve per eseguire codice all'avvio dell'applicazione.
       Implementando IHostedService, ASP.NET Core la chiamerà automaticamente
       durante lo startup (prima di servire richieste HTTP).
     */

    public class SupabaseInitializer : IHostedService
    {
        private readonly Supabase.Client _client;

        //Dato che la classe è un singleton possiamo passare il client di Supabase tramite DI
        public SupabaseInitializer(Supabase.Client client)
        {
            _client = client;
        }
        // Questo metodo viene chiamato da ASP.NET Core all'avvio dell'applicazione
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.InitializeAsync();
        }
        // Questo metodo viene chiamato quando l'applicazione si sta fermando o si blocca
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
