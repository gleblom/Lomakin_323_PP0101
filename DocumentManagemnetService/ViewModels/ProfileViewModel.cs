using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.Windows;

namespace DocumentManagementService.ViewModels
{
    class ProfileViewModel: BaseViewModel
    {
        private readonly Client client;

        private User user;
        public User User 
        { 
            get => user;
            set
            {
                if (user != value) 
                {
                    user = value;
                    OnPropertyChanged(nameof(user));
                }
            }

        }
        public ProfileViewModel()
        {
            client = App.SupabaseService.Client;
            LoadUserInfo();
        }
        public async void LoadUserInfo()  
        {
            if (client.Auth.CurrentUser != null)
            {
                var model = await client.From<User>().
                    Where(x => x.Email == client.Auth.CurrentUser.Email).
                    Get();
                User = model.Model;
            }
        }
    }
}
