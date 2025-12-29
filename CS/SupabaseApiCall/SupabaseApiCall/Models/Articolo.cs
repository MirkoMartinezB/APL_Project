using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SupabaseApiCall.Models
{
    [Table("VARTICOLIMOVIMENTATI")]
    public class Articolo : BaseModel
    {
        [PrimaryKey("CodiceArticolo", false)]
        public string? CodiceArticolo { get; set; }
        [Column("DescrizioneArticolo")]
        public string? DescrizioneArticolo { get; set; }
    }
}
