using Supabase;
using Supabase.Gotrue.Exceptions;
using System.Windows;
namespace DocumentManagemnetService
{
    public class AuthService
    {
        private readonly Client client;

        public AuthService(Client client)
        {
            this.client = client;
        }

        public async Task<bool> SignUpAsync(string email, string password)
        {
            try
            {
                var response = await client.Auth.SignUp(email, password);
                if (response.User != null)
                {
                    return true;
                }
                MessageBox.Show("Неизвестная ошибка регистрации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (GotrueException ex) {

                MessageBox.Show(MapError(ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка сети или сервера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            try
            {
                var response = await client.Auth.SignIn(email, password);
                if (response.User != null)
                {
                    return true;
                }
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (GotrueException ex)
            {
                MessageBox.Show(MapError(ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка сети или сервера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }

        public Supabase.Gotrue.User? GetCurrentUser()
        {
            return client.Auth.CurrentUser;
        }

        public async Task SignOutAsync()
        {
            await client.Auth.SignOut();
        }
        private string MapError(string error)
        {
            if (error.Contains("Password should be"))
                return "Пароль слишком простой. Используйте не менее 6 символов.";
            if (error.Contains("already registered"))
                return "Email уже зарегистрирован.";
            if (error.Contains("Invalid login"))
                return "Неверный email или пароль.";
            if (error.Contains("invalid format"))
                return "Неправильный формат email.";
            return error;
        }
    }

}
