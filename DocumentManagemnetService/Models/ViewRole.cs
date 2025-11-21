using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace DocumentManagementService.Models
{
    [Table("role_view")]
    public class ViewRole : BaseModel, INotifyPropertyChanged
    {
        [Column("role_id")]
        public int Id { get; set; }

        [Column("role")]
        public string Name { get; set; }

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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
