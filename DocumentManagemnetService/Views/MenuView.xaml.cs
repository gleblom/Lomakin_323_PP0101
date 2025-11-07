using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using System.Windows;


namespace DocumentManagementService.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            var navigationService = new NavigationService(MainFrame);
            App.NavigationService = navigationService;
            MenuViewModel vm = new(App.CurrentUser.RoleId);
            DataContext = vm;
            vm.CloseAction ??= new Action(Close);
        }
    }
}
