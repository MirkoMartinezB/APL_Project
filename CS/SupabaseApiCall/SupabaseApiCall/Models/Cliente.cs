using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace SupabaseApiCall.Models
{
    [Table("VCLIENTIMOVIMENTATI")]
    public class Cliente : BaseModel
    {
        [PrimaryKey("CodiceCliente",false)]
        public long CodiceCliente { get; set; }
        [Column("RagioneSociale")]
        public string? RagioneSociale { get; set; } //Nullable string
    }
}
