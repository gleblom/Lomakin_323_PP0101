
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace DocumentManagementService.Models
{
    [Table("units")]
    public class Unit: BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("unit_name")]
        public string UnitName { get; set; }
        [Column("company_id")]
        public int CompanyId { get; set; }

    }
}
