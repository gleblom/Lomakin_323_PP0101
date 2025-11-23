using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;


namespace DocumentManagementService.Models
{
    [Table("profiles")]
    public class User: BaseModel, INotifyPropertyChanged
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
        public int? RoleId { get; set; }

        [Column("phone")]
        public string Telephone { get; set; }
        [Column("company_id")]
        public Guid? CompanyId { get; set; }
        [Column("unit_id")]
        public int? UnitId { get; set; }

        private ViewRole role;
        [IgnoreDataMember]
        public ViewRole Role
        {
            get { return role; }
            set 
            {
                role = value;
                OnPropertyChanged();
            }
        }

        private bool isChecked = true;

        [IgnoreDataMember]
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }

        [Column("verification_code")]
        public int? Code { get; set; }
        [IgnoreDataMember]
        public string Display => $"{SecondName} {FirstName} {ThirdName}";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
