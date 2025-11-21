using DocumentManagementService.ViewModels;

namespace DocumentManagementService.Models
{
    //Модель шага(этапа) для построения графа
    public class RouteStep: BaseViewModel
    {
        public string Id { get; set; }
        public UserView User { get; set; }

        private string role;
        public string Role
        {
            get {  return role; }
            set
            {
                role = value;
                OnPropertyChanged();
            }
        }

        private string name;
        public string Name {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Display));
            }
        }
        private int stepNumber;
        public int StepNumber
        {
            get => stepNumber;
            set
            {
                stepNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Display));
            }
        }

        public string Display => $"Этап {StepNumber} - {Name}";
    }

}
