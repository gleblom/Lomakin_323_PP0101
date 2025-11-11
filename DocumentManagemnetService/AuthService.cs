using DocumentManagementService.Models;
using DocumentManagemnetService;
using DocumentManagemnetService.Views;
using Supabase;
using Supabase.Gotrue.Exceptions;
using System.Windows;
namespace DocumentManagementService
{
    public class AuthService
    {
        private readonly Client client;

        public AuthService()
        {
            client = App.SupabaseService.Client;
        }

        public async Task<bool> SignUpAsync(string email, string password, string firstName, string secondName, string thirdName, string phone, int roleId, int unitId)
        {
            try
            {
                var response = await client.Auth.SignUp(email, password);
                if (response.User != null)
                {

                    var model = await client.From<User>().Get();

                    if (model != null)
                    {

                        await client.Rpc("UpdateProfile", new Dictionary<string, object>
                        {
                            {"FirstName", firstName},
                            {"SecondName", secondName},
                            {"ThirdName", thirdName},
                            {"UserId",  model.Model.Id},
                            {"Phone", phone},
                            {"CompanyId", App.CurrentUser.CompanyId },
                            {"RoleId", roleId},
                            {"UnitId", unitId }

                        });
    
                        return true;
                    }
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
                   await App.SupabaseService.SaveSessionAsync();
                   await client.Auth.SetSession(response.AccessToken, response.RefreshToken);
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

        public async Task SignOutAsync()
        {
            await client.Auth.SignOut();
            App.SupabaseService.DestroySession();
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
            if (error.Contains("missing email or phone"))
                return "Заполните все поля.";
            if (error.Contains("sign-ins are disabled"))
                return "Заполните все поля";
            return error;
        }
    }

}
