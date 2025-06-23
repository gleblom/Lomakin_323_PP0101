using DocumentManagementService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.Models
{
    public class RouteStep: BaseViewModel
    {
        private string name;
        private int stepNumber;
        public string Name {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Display));
            }
        }
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
