using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Data;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;


namespace DocumentManagementService.Models
{
    [Table("profiles")]
    public class User: BaseModel
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
        public string Display => $"{SecondName} {FirstName} {SecondName}";
    }
}
