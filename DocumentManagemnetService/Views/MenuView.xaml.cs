using DocumentManagementService.ViewModels;
using System.Windows;


namespace DocumentManagementService.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            var navigationService = new NavigationService(MainFrame);
            MenuViewModel vm = new(navigationService);
            DataContext = vm;
            vm.CloseAction ??= new Action(Close);
        }
    }
}
