using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class CompanyRegisterViewModel: BaseViewModel
    {
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

        public CompanyRegisterViewModel()
        {
            authService = new AuthService();
            client = App.SupabaseService.Client;
            navigationService = App.NavigationService;

            SignUpCommand = new RelayCommand(SignUp,
                obj => User.FirstName != null && User.SecondName != null &&
                User.FirstName != string.Empty && User.SecondName != string.Empty &&
                User.Email != string.Empty && User.ThirdName != string.Empty &&
                Company.CompanyName != string.Empty &&
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
            bool success = await authService.SignUpAsync(User.Email, Password,
                 User.FirstName, User.SecondName, User.ThirdName,
                 user.Telephone, 3, 0, null);
            
            if (success)
            {
                company.DirectorId = App.RegisteredUser.Id;
                await client.From<Company>().Insert(Company);

                await client.From<User>().Set(x => x.CompanyId, Company.CompanyId).Update();
                //await client.From<Company>().Update(Company);


                MessageBox.Show("Регистрация прошла успешно!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                User = new();
                Company = new();
            }
        }
    }
}
