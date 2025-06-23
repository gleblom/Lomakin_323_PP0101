using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DocumentManagementService.Models
{
    [Table("documents")]
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

        [Column("status")]
        public string Status { get; set; }

        [Column("author_uuid")]
        public string AuthorId { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("route_id")]
        public Guid? RouteId { get; set; }
        [Column("current_step_index")]
        public int CurrentStepIndex { get; set; }
    }

}
