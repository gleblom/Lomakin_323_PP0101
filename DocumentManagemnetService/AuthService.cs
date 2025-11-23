using DocumentManagemnetService;
using NLog;
using Supabase.Gotrue.Exceptions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using Client = Supabase.Client;
using User = DocumentManagementService.Models.User;
namespace DocumentManagementService
{
    public class AuthService
    {
        private readonly Client client;
        private readonly SupabaseService supabase;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AuthService(SupabaseService service)
        { 
            client = service.Client;
            supabase = service;
        }

        public async Task<bool> SignUpAsync(
            string email, string password, string firstName, string secondName, string thirdName, string phone, int roleId, 
            int? unitId, Guid? companyId)
        {
            try
            {
                var response = await client.Auth.SignUp(email, password);
                if (response.User != null)
                {

                    var model = await client.From<User>()
                        .Where(x => x.Email == email).Get();


                    if (model != null)
                    {
                        App.RegisteredUser = model.Model;

                        await client.Rpc("UpdateProfile", new Dictionary<string, object>
                        {
                            {"FirstName", firstName},
                            {"SecondName", secondName},
                            {"ThirdName", thirdName},
                            {"Phone", phone},
                            {"UserId",  model.Model.Id},
                            {"CompanyId",  companyId},
                            {"RoleId", roleId},
                            {"UnitId", unitId }

                        });
                        Logger.Info("Профиль успешно обновлён");
                        return true;
                    }
                }
                MessageBox.Show("Неизвестная ошибка регистрации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (GotrueException ex) {


                Logger.Error(ex);
                MessageBox.Show(MapError(ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show("Ошибка сети или сервера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public static async Task<HttpStatusCode> SendRecoveryCode(string userEmail)
        {
            using var http = new HttpClient();
            var json = JsonSerializer.Serialize(new { email = userEmail});

            var req = new HttpRequestMessage(HttpMethod.Post, "https://kphkeykctqyqgfotrqoy.supabase.co/functions/v1/code-send");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            Logger.Info(res);
            return res.StatusCode;
        }
        public static async Task<HttpStatusCode> SendEmail(string adminToken, string userId, string userPassword)
        {
            using var http = new HttpClient();
            var json = JsonSerializer.Serialize(new {id = userId, password = userPassword });

            var req = new HttpRequestMessage(HttpMethod.Post, "https://kphkeykctqyqgfotrqoy.supabase.co/functions/v1/user-registered");
            req.Headers.Add("Authorization", $"Bearer {adminToken}");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            Logger.Info(res);

            return res.StatusCode;
        }
        public static async Task<HttpStatusCode> ChangeUserPassword(string userId, string newPassword)
        {
            using var http = new HttpClient();
            var json = JsonSerializer.Serialize(new { id = userId, new_password = newPassword });

            var req = new HttpRequestMessage(HttpMethod.Post, "https://kphkeykctqyqgfotrqoy.supabase.co/functions/v1/change_password");
            //req.Headers.Add("Authorization", $"Bearer");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();
            Logger.Info(res);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                MessageBox.Show("Пароль успешно изменен!", "Изменение пароля", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Logger.Info(res);
            return res.StatusCode;
        }
        public async Task<bool> SignInAsync(string email, string password)
        {
            try
            {
                var response = await client.Auth.SignIn(email, password);
                if (response.User != null)
                {
                   await supabase.SaveSessionAsync();
                   await client.Auth.SetSession(response.AccessToken, response.RefreshToken);
                   
                   return true;
                }
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (GotrueException ex)
            {
                Logger.Error(ex);
                MessageBox.Show(MapError(ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show("Ошибка сети или сервера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }

        public async Task SignOutAsync()
        {
            try
            {
                await client.Auth.SignOut();
                supabase.DestroySession();
            }
            catch
            {
                Logger.Info("Something went wrong :(");
            }

        }
        private static string MapError(string error)
        {
            if (error.Contains("Password should be"))
            {
                return "Пароль слишком простой. Используйте не менее 6 символов.";
            }
            if (error.Contains("already registered")) 
            {
                return "Email уже зарегистрирован.";
            }
            if (error.Contains("Invalid login")) 
            {
                return "Неверный email или пароль.";
            }
               
            if (error.Contains("invalid format")) 
            {
                return "Неправильный формат email.";
            }
            if (error.Contains("missing email or phone")) 
            {
                return "Заполните все поля.";
            }
            if (error.Contains("sign-ins are disabled")) 
            { 
                return "Заполните все поля"; 
            }
            return error;
        }
    }

}
