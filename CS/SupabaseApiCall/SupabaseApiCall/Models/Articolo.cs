using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SupabaseApiCall.Models
{
    [Table("VARTICOLIMOVIMENTATI")]
    public class Articolo : BaseModel
    {
        //Tipicamente la scelta degli CodiciArticolo di tipo alfanumerica è dovuta alla necessità di creare articoli con codici parlanti 
        //Identificati ad esempio da una concatenazione di codici e sottocodici ad esempio "AL-VI-RO-0001" 
        //Che identifica la famiglia AL (Alcolici), la sottofamiglia VI (Vini), il gruppo RO (Rosso) e il numero progressivo 0001
        [PrimaryKey("CodiceArticolo", false)]
        public string? CodiceArticolo { get; set; }
        [Column("DescrizioneArticolo")]
        public string? DescrizioneArticolo { get; set; }
    }
}
