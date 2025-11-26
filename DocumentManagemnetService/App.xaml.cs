using DocumentManagementService;
using DocumentManagementService.Models;
using DocumentManagementService.Views;
using NLog;
using NLog.Filters;
using Supabase.Gotrue;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Supabase.Postgrest.Constants;
using User = DocumentManagementService.Models.User;

namespace DocumentManagemnetService
{
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static ViewDocument SelectedDocument { get; set; }
        public static SupabaseService SupabaseService { get; private set; }
        public static User CurrentUser { get; set; }
        public static User RegisteredUser { get; set; }
        public static User SelectedExecutive {  get; set; }
        public static UserView SelectedUser { get; set; }
        public static ObservableCollection<UserView> Users { get; } = [];
        public static INavigationService NavigationService { get; set; }
        private async void LoadUsers()
        {
            Users.Clear();
            var response = await SupabaseService.Client.From<UserView>()
                .Where(x => x.RoleId != 2)
                .Where(x => x.RoleId != 1)
                .Get();
            foreach (var user in response.Models)
            {
                Users.Add(user);
            }

        }
        public static async Task<User?> LoadUserInfo()
        {
            try
            {
                var model = await SupabaseService.Client.From<User>().
                     Where(x => x.Email == SupabaseService.Client.Auth.CurrentUser.Email).
                    Single();
                return model;
            }
            catch (Exception ex)
            {
                Logger.Warn("Не удалось загрузить данные пользователя!");
                Logger.Error(ex.Message);

                return null;
            }

        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var config = new NLog.Config.XmlLoggingConfiguration("NLog.config");

            LogManager.Configuration = config;
            Logger.Info("Приложение запущено.");
            SupabaseService = new SupabaseService();


            await SupabaseService.InitializeAsync();

            await SupabaseService.EnsureSessionIsValidAsync();

            LoadUsers();

            if (SupabaseService.IsAuthenticated)
            {
         
                CurrentUser = await LoadUserInfo();


                new MenuWindow(CurrentUser).Show(); //Если пользователь залогинен открывается главное окно
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
                .Where(x => x.DocumentId == SelectedDocument.Id)
                .Where(x => x.IsApproved == false)
                .Get();

            var approves = docAproves.Models.Where(x => x.ApprovedAt == docAproves.Models.Max(x => x.ApprovedAt)).Single();
            if (SelectedDocument.RouteId != null && SelectedDocument.CurrentStepIndex + 1 == step)
            {
                string comment = approves.Comment;
                MessageBox.Show($"Причина отклонения: {comment}");
            }
            else
            {
                e.Handled = true;
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Info("Приложение завершает работу.");
            LogManager.Shutdown();
            base.OnExit(e);
        }
        private void DoNothing(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
        }

        private void DoNothing(object sender, MouseEventArgs e)
        {
            e.Handled = false;
        }
    }

}
