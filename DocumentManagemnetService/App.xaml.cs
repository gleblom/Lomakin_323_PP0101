using DocumentManagementService;
using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService.Views;
using Supabase;
using System.Windows;

namespace DocumentManagemnetService
{
    public partial class App : Application
    {
        public static ViewDocument SelectedDocument { get; set; }
        public static SupabaseService SupabaseService { get; private set; }
        public static User CurrentUser { get; set; }
        public static UserView SelectedUser { get; set; }
        public static INavigationService NavigationService { get; set; }  
        public async Task<User?> LoadUserInfo()
        {
            var model = await SupabaseService.Client.From<User>().
                 Where(x => x.Email == SupabaseService.Client.Auth.CurrentUser.Email).
                 Get();
            var models = model.Models;
            return model.Model;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SupabaseService = new SupabaseService();

            await SupabaseService.InitializeAsync();

            await SupabaseService.EnsureSessionIsValidAsync(); 

            if (SupabaseService.IsAuthenticated) 
            {
                CurrentUser = await LoadUserInfo();
                new MenuWindow().Show(); //Если пользователь залогинен открывается главное окно
            }
            else
            {
                new LoginView().Show(); //если нет - окно входа
            }

        }
        public static bool IsWindowOpen<T>(string name = "") where T : Window //Проверка, открыто ли окно
        {
            return string.IsNullOrEmpty(name)
               ? Current.Windows.OfType<T>().Any()
               : Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

    }

}
