using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace DocumentManagementService.Models
{
    [Table("notifications")]
    public class Notification: BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("document_id")]
        public int DocumentId { get; set; }
    }
}
