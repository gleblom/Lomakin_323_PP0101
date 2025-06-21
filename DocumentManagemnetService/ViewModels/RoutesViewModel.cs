using DocumentManagementService.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RoutesViewModel: BaseViewModel
    {
        public ICommand CreateRouteCommand { get; }
        public RoutesViewModel() {
            CreateRouteCommand = new RelayCommand(OpenRoutesEditorWindow);
        }
        private void OpenRoutesEditorWindow()
        {
            var window = new RouteEditorWindow()
            {
                DataContext = new RouteEditorViewModel()
            };
            window.Show();
        }
    }
}
