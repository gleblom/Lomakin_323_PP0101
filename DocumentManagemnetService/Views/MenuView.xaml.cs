using DocumentManagementService.Models;
using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using NLog;
using System.Windows;


namespace DocumentManagementService.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow(User currentUser)
          {
            InitializeComponent();
            var navigationService = new NavigationService(MainFrame);
            App.NavigationService = navigationService;
            int? role = currentUser?.RoleId; 
            MenuViewModel vm = new(role);
            DataContext = vm;
            vm.CloseAction ??= new Action(Close);
        }
    }
}
