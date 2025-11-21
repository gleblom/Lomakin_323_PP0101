using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using System.Windows;

namespace DocumentManagementService.Views
{
    public partial class StartupView : Window
    {
        public StartupView()
        {
            InitializeComponent();
            INavigationService navigationService = new NavigationService(StartupFrame);
            App.NavigationService = navigationService;
            DataContext = new StartupViewModel(navigationService);
        }
    }
}
