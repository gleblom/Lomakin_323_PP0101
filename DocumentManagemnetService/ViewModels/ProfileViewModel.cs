using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;

namespace DocumentManagementService.ViewModels
{
    class ProfileViewModel: BaseViewModel
    {
        private readonly Client client;

        private User user;
        public User User 
        { 
            get {  return user; }
            set
            {
                if (user != value) 
                {
                    user = value;
                    OnPropertyChanged();
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
