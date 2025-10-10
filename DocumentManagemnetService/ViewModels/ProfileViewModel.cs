using DocumentManagemnetService;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.ViewModels
{
    class ProfileViewModel: BaseViewModel
    {
        private readonly Client client;
        public string Name { get; set; }


        public ProfileViewModel()
        {
            client = App.SupabaseService.Client;
        }
    }
}
