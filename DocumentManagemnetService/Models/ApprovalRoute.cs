using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace DocumentManagementService.Models
{
    [Table("approval_routes")]
    public class ApprovalRoute : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("graph")]
        public string GraphJson { get; set; }

        [Column("created_by")]
        public string CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


    }

}
