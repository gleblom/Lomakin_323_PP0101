using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DocumentManagementService.Models
{
    [Table("users_view")]
    public class UserView: User
    {
        [Reference(typeof(Unit))]
        public Unit? Unit { get; set; }

        [Reference(typeof(Role))]
        public Role? Role { get; set; }

        [Reference(typeof(Company))]
        public Company? Company { get; set; }       
    }
}
