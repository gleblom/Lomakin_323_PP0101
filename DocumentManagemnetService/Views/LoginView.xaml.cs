using DocumentManagementService.Views;
using Supabase;
using System.Windows;


namespace DocumentManagemnetService.Views
{
    public partial class LoginView : Window
    {
        private readonly AuthService auth;

        public LoginView(Client client)
        {
            InitializeComponent();
            auth = new AuthService(client);
        }

        private async void SignUp_ClickAsync(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text;
            var pass = PasswordBox.Text;
            
            if (await auth.SignUpAsync(email, pass))
            {
                MessageBox.Show("Проверьте почту для подтверждения.");
            }
        }

        private async void SignIn_ClickAsync(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text;
            var pass = PasswordBox.Text;

            var session = await auth.SignInAsync(email, pass);

            if (session)
            {
                MessageBox.Show("Успешный вход!");
                var mainWindow = new MenuWindow();
                mainWindow.Show();

                GetWindow(this)?.Close();
            }
        }
    }
}
