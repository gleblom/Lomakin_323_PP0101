using DocumentManagementService.Models;
using DocumentManagemnetService;
using NLog;
using Supabase;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class CompanyRegisterViewModel: BaseViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AuthService authService;
        private readonly Client client;
        private readonly INavigationService navigationService;
        public ICommand SignUpCommand { get; }
        public ICommand SignInCommand { get; }
        public ICommand RecoveryCommand { get; }

        private Company company;
        public Company Company
        {
            get { return company; }
            set
            {
                company = value;
                OnPropertyChanged();
            }
        }
        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }
        private UserView user;
        public UserView User
        {
            get { return user; }
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        public const string BasicTextPattern = @"^[a-zA-Zа-яА-Я0-9\s\.,!?;:""'\(\)\-]*$";

        public CompanyRegisterViewModel()
        {
            client = App.SupabaseService.Client;
            navigationService = App.NavigationService;
            authService = new AuthService(App.SupabaseService);

           
            var regex = new Regex(BasicTextPattern, RegexOptions.Compiled);

            SignUpCommand = new RelayCommand(SignUp,
                obj => User.FirstName != null && User.SecondName != null &&
                User.FirstName != string.Empty && User.SecondName != string.Empty &&
                User.Email != string.Empty && User.ThirdName != string.Empty &&
                Company.CompanyName != string.Empty && regex.IsMatch(Company.CompanyName) &&
                regex.IsMatch(password) && IsEnabled &&
                User.ThirdName != null && User.Email != null && User.Telephone != null
                && User.Telephone?.Length == 10 && Company.CompanyName != null
                && Password != string.Empty && Password?.Length > 5);


            SignInCommand = new RelayCommand(SignIn);
            RecoveryCommand = new RelayCommand(Recovery);

            Company = new()
            {
                CompanyId = Guid.NewGuid()
            };


            User = new()
            {
                CompanyId = Company.CompanyId,
            };
   


        }
        private void Recovery()
        {
            navigationService.Navigate("Recover");
        }
        private void SignIn()
        {
            navigationService.Navigate("Login");
        }
        private async void SignUp()
        {
            IsEnabled = false;
            try
            {
                bool success = await authService.SignUpAsync(User.Email, Password,
                    User.FirstName, User.SecondName, User.ThirdName,
                     user.Telephone, 3, 0, null);


                if (success)
                {
                    Logger.Info("Регистрация пользователя прошла успешно");
                    company.DirectorId = App.RegisteredUser.Id;
                    await client.From<Company>().Insert(Company);

                    var update = await client.From<User>().Where(x => x.Email == User.Email).Set(x => x.CompanyId, Company.CompanyId).Update();


                    MessageBox.Show("Регистрация прошла успешно!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                    User = new();
                    Company = new();
                    Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            IsEnabled = true;
        }
    }
}
