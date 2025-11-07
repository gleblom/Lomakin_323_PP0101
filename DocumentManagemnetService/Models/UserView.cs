using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DocumentManagementService.Models
{
    [Table("users_view")]
    public class UserView: User
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("second_name")]
        public string SecondName { get; set; }

        [Column("third_name")]
        public string ThirdName { get; set; }

        [Column("email")]
        public string Email { get; set; }
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("phone")]
        public string Telephone { get; set; }
        [Column("company_id")]
        public Guid CompanyId { get; set; }
        [Column("unit_id")]
        public int UnitId { get; set; }

        [Reference(typeof(Unit))]
        public Unit? Unit { get; set; }

        [Reference(typeof(Role))]
        public Role? Role { get; set; }

        [Reference(typeof(Company))]
        public Company? Company { get; set; }
        public string Display => $"{SecondName} {FirstName} {ThirdName}";
    }
}
