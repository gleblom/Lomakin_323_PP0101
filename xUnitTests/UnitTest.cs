
using DocumentManagementService;
using System.Net;
using System.Threading.Tasks;

namespace xUnitTests
{
    public class UnitTest
    {
        private static readonly SupabaseService service = new SupabaseService();
        private static readonly AuthService authService = new AuthService(service);
        [Fact(DisplayName = "1. Регистрация - успех")]
        public async void SignUpSuccess()
        {
            string email = "test@email.com";
            string password = "password";
            string name = "name";
            bool success = await authService
                .SignUpAsync(email, password, name, name, name, "45464647", 4, null, null);
            Assert.True(success);
            
        }
        [Fact(DisplayName = "2. Регистрация - провал")]
        public async void SignUpFail()
        {
            string email = "lomakgleb@yandex.ru";
            HttpStatusCode code = await AuthService.SendRecoveryCode(email);
            Assert.Equal(HttpStatusCode.OK, code);
        }
        [Fact(DisplayName = "3. Вход - успех")]
        public async void SignInSuccess()
        {
            string email = "test@email.com";
            string password = "password";
            bool success = await authService.SignInAsync(email, password);
            Assert.True(success);
        }
        [Fact(DisplayName = "4. Вход - провал")]
        public async void SignInFail()
        {
            string email = "lomakgleb@yandex.ru";
            HttpStatusCode code = await AuthService.SendRecoveryCode(email);
            Assert.Equal(HttpStatusCode.OK, code);

        }
        [Fact(DisplayName = "5. Смена пароля")]
        public async void ChangePassword()
        {
            string userId = "785cb3c7-0729-4cab-87b3-cd1609fe52de";
            string password = "password";
            HttpStatusCode code = await AuthService.ChangeUserPassword(userId, password);
            Assert.Equal(HttpStatusCode.OK, code);
        }
        [Fact(DisplayName = "6. Отправка кода восстановления")]
        public async void SendCode()
        {
            string email = "lomakgleb@yandex.ru";
            HttpStatusCode code = await AuthService.SendRecoveryCode(email);
            Assert.Equal(HttpStatusCode.OK, code);
        }
    }
}