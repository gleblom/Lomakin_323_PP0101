using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RecoveryViewModel : BaseViewModel
    {
        private readonly INavigationService navigationService;
        private readonly Client client;
        public ICommand GetCodeCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand SaveCommand { get; }

        private bool isEnabledChanger = true;
        public bool IsEnabledChanger
        {
            get { return isEnabledChanger; }
            set
            {
                isEnabledChanger = value;
                OnPropertyChanged();
            }
        }

        private bool isEnabledRecovery = true;
        public bool IsEnabledRecovery
        {
            get { return isEnabledRecovery; }
            set
            {
                isEnabledRecovery = value;
                OnPropertyChanged();
            }
        }

        private string recoveryCode;
        public string RecoveryCode
        {
            get { return recoveryCode; }
            set
            {
                recoveryCode = value;
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
        private string recoveryPassword;
        public string RecoveryPassword
        {
            get { return recoveryPassword; }
            set
            {
                recoveryPassword = value;
                OnPropertyChanged();
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        private Visibility emailVisibility;
        public Visibility EmailVisibility
        {
            get { return emailVisibility; }
            set
            {
                emailVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility recoveryVisibility = Visibility.Collapsed;
        public Visibility RecoveryVisibility
        {
            get { return recoveryVisibility; }
            set
            {
                recoveryVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility passwordVisibility = Visibility.Collapsed;
        public Visibility PasswordVisibility
        {
            get { return passwordVisibility; }
            set
            {
                passwordVisibility = value;
                OnPropertyChanged();
            }
        }
        public RecoveryViewModel()
        {
            navigationService = App.NavigationService;
            client = App.SupabaseService.Client;

            GetCodeCommand = new RelayCommand(GetCode, obj => email != null);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            SaveCommand = new RelayCommand(Save);
        }
        private async void GetCode()
        {

            HttpStatusCode status = await AuthService.SendRecoveryCode(email);
            if (status == HttpStatusCode.OK)
            {
                MessageBox.Show("На указанный Email был отправлен код для восстановления", "Восстановление", MessageBoxButton.OK, MessageBoxImage.Information);
                RecoveryVisibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Произошла непредвиденная ошибка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private async void ChangePassword()
        {
            var user = await client.From<User>()
                .Where(x => x.Email == email).Get();
            int? code = user.Model.Code;

            if (code == int.Parse(recoveryCode))
            {
                PasswordVisibility = Visibility.Visible;
                IsEnabledRecovery = false;
                IsEnabledChanger = false;
            }
            else
            {
                MessageBox.Show("Неверный код восстановления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private async void Save()
        {
            if (password == recoveryPassword)
            {
                var user = await client.From<User>()
                    .Where(x => x.Email == email)
                    .Single();
                await AuthService.ChangeUserPassword(user.Id.ToString(), Password);
                MessageBox.Show("Ваш пароль успешно изменен!", "Изменение пароля", MessageBoxButton.OK, MessageBoxImage.Information);
                navigationService.Navigate("Login");
                user.Code = null;
                await user.Update<User>();
            }
            else
            {
                MessageBox.Show("Пароли должны совпадать!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
