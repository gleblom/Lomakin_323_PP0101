using DocumentManagementService.Views;
using DocumentManagemnetService.Views;
using Supabase;
using System.Diagnostics;
using System.Threading.Tasks;
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

            SupabaseService.Client.Auth.LoadSession();


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
