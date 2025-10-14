using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var model = await client.From<User>().Get();
                User = model.Model;
            }
        }
    }
}
