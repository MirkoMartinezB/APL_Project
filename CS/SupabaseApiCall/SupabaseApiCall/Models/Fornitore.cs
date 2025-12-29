using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace SupabaseApiCall.Models
{
    [Table("VFORNITORIMOVIMENTATI")]
    public class Fornitore : BaseModel
    {
        [PrimaryKey("CodiceFornitore",false)]
        public long CodiceFornitore { get; set; }
        [Column("RagioneSociale")]
        public string? RagioneSociale { get; set; } //Nullable string
    }
}
