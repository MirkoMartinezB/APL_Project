namespace SupabaseApiCall.Contracts.Anagrafiche
{
    /* La classe ArticoloResponse rappresenta la risposta per un articolo.
     * Viene utilizzata per restituire le informazioni di base di un articolo, come il codice e la descrizione.
     * Viene popolata a partire dai dati memorizzati nel database Supabase.
     */
    public class ArticoloResponse
    {
        //Tipicamente la scelta degli CodiciArticolo di tipo alfanumerica è dovuta alla necessità di creare articoli con codici parlanti 
        //Identificati ad esempio da una concatenazione di codici e sottocodici ad esempio "AL-VI-RO-0001" 
        //Che identifica la famiglia AL (Alcolici), la sottofamiglia VI (Vini), il gruppo RO (Rosso) e il numero progressivo 0001
        public string? CodiceArticolo { get; set; }
        public string? DescrizioneArticolo { get; set; }
    }
}
