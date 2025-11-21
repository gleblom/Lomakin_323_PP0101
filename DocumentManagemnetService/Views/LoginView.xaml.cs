using DocumentManagementService;
using DocumentManagementService.ViewModels;

using System.Windows.Controls;


namespace DocumentManagemnetService.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            LoginViewModel vm = new();
            DataContext = vm;
        }
    }
}
