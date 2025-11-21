using DocumentManagementService.Views;
using DocumentManagemnetService;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class LoginViewModel: BaseViewModel
    {
        private readonly AuthService auth;
        private readonly INavigationService navigationService;

        //Привязка текста для Textbox'ов
        public string SignInEmail { get; set; }
        public string SignInPassword { get; set; }
        public ICommand SignUpCommand { get; }
        public ICommand SignInCommand { get; }
        public LoginViewModel() 
        {
            auth = new AuthService();
            navigationService = App.NavigationService;

            SignInCommand = new RelayCommand(SignIn);
            SignUpCommand = new RelayCommand(SignUp);
        }
        private void SignUp()
        {
            navigationService.Navigate("SignUp");
        }
        private async void SignIn()
        {
            
            var session = await auth.SignInAsync(SignInEmail, SignInPassword);


            if (session)
            {
                MenuWindow window = new();
                Application.Current.MainWindow.Close();
                App.Current.MainWindow = window;
                App.Current.MainWindow.Show();
                MessageBox.Show("Успешный вход!");

            }
        }
    }
}
