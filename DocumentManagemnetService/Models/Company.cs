using Supabase.Postgrest.Models;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace DocumentManagementService.Models
{
    [Table("companies")]
    public class Company: BaseModel
    {
        [Column("company_id")]
        public Guid? CompanyId { get; set; }
        [Column("company_name")]
        public string CompanyName { get; set; }
        [Column("director")]
        public Guid DirectorId { get; set; }
    }
}
