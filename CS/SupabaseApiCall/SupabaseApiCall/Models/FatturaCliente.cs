using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace SupabaseApiCall.Models
{
    [Table("VFATTURECLIENTI")]
    public class FatturaCliente : BaseModel
    {
        [Column("CodiceCliente")]
        public long CodiceCliente { get; set; }
        [Column("DataFattura")]
        public DateTime DataFattura { get; set; }
        [Column("NumeroFattura")]
        public long NumeroFattura { get; set; }
        [Column("CodiceArticolo")]
        public String? CodiceArticolo { get; set; }      //Nullable string  
        [Column("QtaMovimento")]
        public double QtaMovimento { get; set; }
        [Column("Prezzo")]
        public double Prezzo { get; set; }
        [Column("ScontoPerc")]
        public double ScontoPerc { get; set; }
        [Column("Importo")]
        public double Importo { get; set; }
        [Column("ImportoIvato")]
        public double ImportoIvato { get; set; }
        [Column("ImportoIva")]
        public double ImportoIva { get; set; }
        [Column("AliquotaIva")]
        public int AliquotaIva { get; set; }
    }
}
