using DocumentManagementService;
using DocumentManagementService.Models;
using DocumentManagementService.Views;
using NLog;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Supabase.Postgrest.Constants;

namespace DocumentManagemnetService
{
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static ViewDocument SelectedDocument { get; set; }
        public static SupabaseService SupabaseService { get; private set; }
        public static User CurrentUser { get; set; }
        public static User RegisteredUser { get; set; }
        public static UserView SelectedUser { get; set; }
        public static INavigationService NavigationService { get; set; }
        public static async Task<User?> LoadUserInfo()
        {
            var model = await SupabaseService.Client.From<User>().
                 Where(x => x.Email == SupabaseService.Client.Auth.CurrentUser.Email).
                 Single();
            return model;

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SupabaseService = new SupabaseService();

            await SupabaseService.InitializeAsync();

            await SupabaseService.EnsureSessionIsValidAsync();



            if (SupabaseService.IsAuthenticated)
            {
                Logger.Info("Запуск приложения");
                CurrentUser = await LoadUserInfo();
                while(CurrentUser == null)
                {
                    Logger.Info("Ожидание информации о пользователе...");
                }

                new MenuWindow().Show(); //Если пользователь залогинен открывается главное окно
            }
            else
            {
                new StartupView().Show(); //если нет - окно входа
            }

        }
        public static bool IsWindowOpen<T>(string name = "") where T : Window //Проверка, открыто ли окно
        {
            return string.IsNullOrEmpty(name)
               ? Current.Windows.OfType<T>().Any()
               : Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        private async void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            var step = Convert.ToInt32(border.Tag);
            
            var docAproves = await SupabaseService.Client.From<DocumentApprovals>()
                .Where(x => x.StepIndex == step)
                .Where(x => x.DocumentId == SelectedDocument.Id)
                .Order("id", Ordering.Descending)
                .Get();
            if (SelectedDocument.RouteId != null && SelectedDocument.CurrentStepIndex == step)
            {
                string comment = docAproves.Models[0].Comment;
                MessageBox.Show($"Причина отклонения: {comment}");
            }
            else
            {
                e.Handled = true;
            }
        }
    }

}
