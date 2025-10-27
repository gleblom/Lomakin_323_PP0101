using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace DocumentManagementService.Models
{
    [Table("categories")]
    public class Category: BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("category")]
        public string CategoryName {  get; set; }
        private bool isChecked = true;
        public bool IsChecked 
        {
            get {  return isChecked; }
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
