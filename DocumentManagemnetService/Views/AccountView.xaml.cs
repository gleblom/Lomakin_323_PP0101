using DocumentManagementService.ViewModels;
using System.Windows.Controls;


namespace DocumentManagementService.Views
{

    public partial class AccountView : UserControl
    {
        public AccountView()
        {
            InitializeComponent();
            NavigationService navigationService = new(AccountFrame);
            AccountViewModel vm = new(navigationService);
            DataContext = vm;

        }
    }
}
