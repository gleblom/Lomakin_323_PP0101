using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DocumentManagemnetService.Models
{
    public class Document : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }

}
