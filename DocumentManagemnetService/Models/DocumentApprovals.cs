using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace DocumentManagementService.Models
{
    [Table("document_approvals")]
    public class DocumentApprovals : BaseModel
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("document_id")]
        public int DocumentId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("step_index")]
        public int StepIndex { get; set; }

        [Column("approved")]
        public bool IsApproved { get; set; }
        [Column("approved_at")]
        public DateTime ApprovedAt {  get; set; }
        [Column("comment")]
        public string? Comment { get; set; }
    }
}
