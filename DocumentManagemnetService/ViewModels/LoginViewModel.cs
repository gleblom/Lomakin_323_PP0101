using DocumentManagementService.Views;
using DocumentManagemnetService;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class LoginViewModel: BaseViewModel
    {
        private readonly AuthService auth;
        public Action CloseAction { get; set; } //Делегат для связи модели представления и code-behind. Закрывает LoginView при переходе в главное окно.

        //Привязка текста для Textbox'ов
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string SignInEmail { get; set; }
        public string SignInPassword { get; set; }
        public ICommand SignUpCommand { get; }
        public ICommand SignInCommand { get; }
        public LoginViewModel() 
        {
            SignUpCommand = new RelayCommand(SignUp, obj => 
            Name != null && 
            SecondName != null 
            && ThirdName != null);

            SignInCommand = new RelayCommand(SignIn);

            auth = new AuthService(App.SupabaseService.Client);
        }
        private async void SignUp()
        {
            if (await auth.SignUpAsync(Email, Password, Name, SecondName, ThirdName))
            {
                Email = string.Empty;
                Password = string.Empty;
                Name = string.Empty;
                SecondName = string.Empty;
                ThirdName = string.Empty;
                MessageBox.Show("Проверьте почту для подтверждения.");
            }
        }
        private async void SignIn()
        {
            var session = await auth.SignInAsync(SignInEmail, SignInPassword);

            if (session)
            {
                MessageBox.Show("Успешный вход!");
                Application.Current.MainWindow = new MenuWindow();
                Application.Current.MainWindow.Show();
                CloseAction();
            }
        }
    }
}
