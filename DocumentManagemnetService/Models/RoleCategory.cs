using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.Models
{
    [Table("roles_categories")]
    public class RoleCategory: BaseModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("category_id")]
        public int CategoryId { get; set; }
        [Column("company_id")]
        public Guid CompanyId  { get; set; }

        [Reference(typeof(Role))]
        public Role Role { get; set; }

        [Reference(typeof(Category))]
        public Category Category { get; set; }
    }
}
