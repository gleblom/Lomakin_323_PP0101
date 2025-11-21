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
            int? role = App.CurrentUser.RoleId;
            MenuViewModel vm = new(role);
            DataContext = vm;
            vm.CloseAction ??= new Action(Close);
        }
    }
}
