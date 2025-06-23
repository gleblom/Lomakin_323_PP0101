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
                new MenuWindow().Show();
            }
            else
            {
                new LoginView(SupabaseService.Client).Show(); 
            }

        }

    }

}
