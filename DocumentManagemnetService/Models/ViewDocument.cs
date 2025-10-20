using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;


namespace DocumentManagementService.Models
{
    [Table("documents_view")]
    public class ViewDocument: BaseModel
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
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("second_name")]
        public string SecondName { get; set; }

        [Column("third_name")]
        public string ThirdName { get; set; }
        public string FullName => $"{SecondName} {FirstName} {ThirdName}";
    }
}
