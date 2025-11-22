using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using NLog;
using System.Windows;


namespace DocumentManagementService.Views
{
    public partial class MenuWindow : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public MenuWindow()
          {
            InitializeComponent();
            var navigationService = new NavigationService(MainFrame);
            App.NavigationService = navigationService;

            while(App.CurrentUser == null)
            {
                Logger.Info("Загрузка..");
            }
            int? role = App.CurrentUser.RoleId;
            MenuViewModel vm = new(role);
            DataContext = vm;
            vm.CloseAction ??= new Action(Close);
        }
    }
}
