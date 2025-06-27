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
        public Action CloseAction { get; set; }
        public string SelectedFileCategory { get; set; }
        public ApprovalRoute? SelectedRoute { get; set; }
        public ObservableCollection<ApprovalRoute> Routes { get; } = [];
        public ICommand SaveDocumentCommand { get; }
        public ICommand CancelCommand { get; }
        public SelectRouteViewModel()
        {
            client = App.SupabaseService.Client;
            SaveDocumentCommand = new RelayCommand(SaveDocument, obj => SelectedRoute != null);
            CancelCommand = new RelayCommand(CloseAction);
            LoadRoutes();

        }
        private async void SaveDocument()
        {
            bool success = await documentService.AddDocumentAsync(DocumentTitle, SelectedFileCategory, "В процессе согласования", SelectedFilePath, SelectedRoute);
            if (success)
            {
                MessageBox.Show("Документ сохранен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            CloseAction();
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
