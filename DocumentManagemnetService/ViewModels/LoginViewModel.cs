using DocumentManagementService.Views;
using DocumentManagemnetService;
using NLog;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class LoginViewModel: BaseViewModel
    {
        private readonly AuthService auth;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly INavigationService navigationService;

        //Привязка текста для Textbox'ов
        public string SignInEmail { get; set; }
        public string SignInPassword { get; set; }
        public ICommand SignUpCommand { get; }
        public ICommand SignInCommand { get; }
        public LoginViewModel() 
        {
            auth = new AuthService(App.SupabaseService);
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
                App.CurrentUser = await App.LoadUserInfo();
                MenuWindow window = new(App.CurrentUser);
                Application.Current.MainWindow.Close();
                App.Current.MainWindow = window;
                App.Current.MainWindow.Show();
                MessageBox.Show($"Успешный вход как {App.CurrentUser.UserRole.Name}!");
                Logger.Info($"Выполняется вход пользователя {App.CurrentUser.Email}(id {App.SupabaseService.Client.Auth.CurrentUser.Id})...");
                Logger.Info($"Выполненен вход пользователя {App.CurrentUser.Email} ({App.CurrentUser.SecondName} {App.CurrentUser.FirstName} {App.CurrentUser.ThirdName})");
                Logger.Info($"Пользователь определен как {App.CurrentUser.UserRole.Name}");
                Logger.Info($"Сессия успешно сохранена локально");
                Logger.Info($"Выполняется переход на страницу {App.CurrentUser.UserRole.Name}a");


            }
        }
    }
}
