using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using Supabase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RoutesViewModel : BaseViewModel
    {
        private readonly Client client;
        public ICommand CreateRouteCommand { get; }
        public ObservableCollection<ApprovalRoute> Routes { get; } = [];
        public ApprovalRoute? SelectedRoute { get; set; }
        public ICommand EditRouteCommand { get; }

        public RoutesViewModel() {
            CreateRouteCommand = new RelayCommand(ConfirmSelection);
            EditRouteCommand = new RelayCommand(OpenRoutesEditorWindow, obj => SelectedRoute != null);

            client = App.SupabaseService.Client;

            LoadRoutes();
        }
        private void OpenRoutesEditorWindow()
        {
            var window = new RouteEditorWindow()
            {
                DataContext = new RouteEditorViewModel(App.SupabaseService.Client, SelectedRoute)
            };
            window.Show();
        }

        private async void LoadRoutes()
        {
            var response = await client
                .From<ApprovalRoute>()
                .Select("*")
                .Get();

            Routes.Clear();
            foreach (var route in response.Models)
            {
                Routes.Add(route);
            }
        }
        private void ConfirmSelection() 
        {
            var window = new RouteEditorWindow()
            {
                DataContext = new RouteEditorViewModel(App.SupabaseService.Client)
            };
            window.Show();
        }
    }
}
