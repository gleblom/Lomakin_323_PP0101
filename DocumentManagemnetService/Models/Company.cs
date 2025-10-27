using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace DocumentManagementService.Models
{
    [Table("companies")]
    public class Company: BaseModel
    {
        [PrimaryKey("company_id")]
        public int CompanyId { get; set; }
        [Column("company_name")]
        public int CompanyName { get; set; }
        [Column("director")]
        public int DirectorId { get; set; }
    }
}
