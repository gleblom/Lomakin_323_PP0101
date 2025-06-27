using DocumentManagementService;
using DocumentManagementService.ViewModels;
using DocumentManagementService.Views;
using Supabase;
using System.Windows;


namespace DocumentManagemnetService.Views
{
    public partial class LoginView : Window
    {
        private readonly AuthService auth;
        public LoginView()
        {
            InitializeComponent();
            LoginViewModel vm = new();
            DataContext = vm;
            vm.CloseAction ??= new Action(Close);
        }
    }
}
