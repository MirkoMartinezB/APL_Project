using SupabaseApiCall.Contracts.Anagrafiche;

namespace SupabaseApiCall.Contracts.DatiIntermedi
{
    /* La classe FatturatoMensileResponse rappresenta la risposta per il fatturato mensile di un cliente (o fornitore).
     * Viene utilizzata per restituire il totale del fatturato per ogni mese in un determinato intervallo di tempo.
     * Viene popolata a partire dai dati delle fatture elaborate nel servizio FattureService.
     * Viene sfruttata la potenzialità di LINQ per raggruppare e sommare i dati in modo efficiente.
     */
    public class FatturatoMensileResponse
    {
        
        public int Anno { get; set; }
        public int Mese { get; set; }
        public double Fatturato { get; set; } // Totale fatturato per il mese
    }
}
