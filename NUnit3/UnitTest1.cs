using DocumentManagementService;
using DocumentManagementService.Views;
using System.Net;
using System.Threading.Tasks;

namespace NUnit3
{
    public class Tests
    {
        private AuthService authService;
        [SetUp]
        public void Setup()
        {
            SupabaseService supabaseService = new SupabaseService();
            authService = new AuthService(supabaseService);
        }

        [Test]
        public async Task SignUpTestSuccess()
        {

            string email = "test@email.com";
            string password = "password";
            string name = "Test";
            string phone = "1234567890";
            bool success = await authService
                .SignUpAsync(email, password, name, name, name, phone, 4, null, null);
            Assert.IsTrue(success);
        }
        [Test]
        public async Task SignUpTestFailAsync()
        {

            string email = "435 rt43fg";
            string password = "password";
            string name = "Test";
            string phone = "1234567890";
            bool success = await authService
                .SignUpAsync(email, password, name, name, name, phone, 4, null, null);
            Assert.IsTrue(!success);
        }
        [Test]
        public async Task SignInTestFail()
        {
            string email = "fail.com";
            string password = "password";
            bool success = await authService.SignInAsync(email, password);
            Assert.IsTrue(!success);
        }
        [Test]
        public async Task SignInTestSuccessAsync()
        {
            string email = "test@email.com";
            string password = "password";
            bool success = await authService.SignInAsync(email, password);
            Assert.IsTrue(success);
        }
        [Test]
        public async Task ChangePassword()
        {
            string userId = "cdbdecf0-64b3-423e-a420-f019afe28040";
            string password = "password";
            HttpStatusCode code = await AuthService.ChangeUserPassword(userId, password);
            Assert.AreEqual(HttpStatusCode.OK, code);
        }
        [Test]
        public async Task SendRecoveryCode()
        {
            string email = "test@email.com";
            HttpStatusCode code = await AuthService.SendRecoveryCode(email);
            Assert.AreEqual(HttpStatusCode.OK, code);
        }
    }
}