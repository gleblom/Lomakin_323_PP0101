using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class SelectRouteViewModel: BaseViewModel
    {
        private readonly Client client;
        public DocumentService documentService;
        public string DocumentTitle { get; set; }
        public string SelectedFilePath { get; set; }
        public string SelectedFileCategory { get; set; }
        public ApprovalRoute? SelectedRoute { get; set; }
        public ObservableCollection<ApprovalRoute> Routes { get; } = [];
        public ICommand SaveRouteCommand { get; }
        public SelectRouteViewModel()
        {
            client = App.SupabaseService.Client;
            SaveRouteCommand = new RelayCommand(SaveRoute, obj => SelectedRoute != null);
            LoadRoutes();

        }
        private async void SaveRoute()
        {
            bool success = await documentService.AddDocumentAsync(DocumentTitle, SelectedFileCategory, "На согласовании", SelectedFilePath, SelectedRoute.Id);
            if (success)
            {
                MessageBox.Show("Документ сохранен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
    }
}
