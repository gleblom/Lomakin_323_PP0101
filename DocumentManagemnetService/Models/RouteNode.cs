

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace DocumentManagementService.Models
{
    //Модель кастомного узла для графа
    public class RouteNode: INotifyPropertyChanged
    {
        public int StepNumber { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public UserView User { get; set; }
        public string Role {  get; set; }

        private SolidColorBrush brushes = Brushes.LightGray;


        public SolidColorBrush NodeColour
        {
            get {  return brushes; }
            set { brushes = value; }
        }
        public string Display => $"{StepNumber}\n({Name})";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
             => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
}
