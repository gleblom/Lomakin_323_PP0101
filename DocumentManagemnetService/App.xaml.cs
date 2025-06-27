using DocumentManagementService;
using DocumentManagementService.Views;
using DocumentManagemnetService.Views;
using System.Windows;

namespace DocumentManagemnetService
{
    public partial class App : Application
    {
        public static SupabaseService SupabaseService { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SupabaseService = new SupabaseService();

            await SupabaseService.InitializeAsync();

            await SupabaseService.EnsureSessionIsValidAsync(); 

            if (SupabaseService.IsAuthenticated) 
            {
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
