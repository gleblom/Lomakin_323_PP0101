using DocumentManagementService.Models;
using DocumentManagemnetService;
using Microsoft.Win32;
using Supabase;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using static Supabase.Postgrest.Constants;

namespace DocumentManagementService.ViewModels
{
    public class PublicDocumentsViewModel : BaseViewModel
    {
        private readonly Client client;
        private readonly INavigationService navigationService;
        public string SearchQuery { get; set; }
        public ObservableCollection<ViewDocument> FilteredDocuments { get; } = [];
        public ICommand SearchCommand { get; }
        public ObservableCollection<MenuItemModel> ItemsSource { get; }
        public ICommand DownloadCommand { get; }

        private ViewDocument selectedDocument;
        public ViewDocument SelectedDocument 
        { 
            get { return selectedDocument; }
            set
            {
                if (selectedDocument != value)
                {

                    selectedDocument = value;
                    OnPropertyChanged();

                    App.SelectedDocument = SelectedDocument;
                    navigationService.Navigate("Viewer"); //Когда пользователь выбирает пункт меню, вызывается переход
                }
            }
        }

        public PublicDocumentsViewModel(INavigationService navigationService)
        { 
            this.navigationService = navigationService; 
            client = App.SupabaseService.Client;
            SearchCommand = new RelayCommand(Search, 
                obj => SearchQuery != null);
            DownloadCommand = new RelayCommand(Download, obj => SelectedDocument !=null);
            LoadDocuments();
        }
        private async void LoadDocuments()
        {
            FilteredDocuments.Clear();
            var documents = await client.From<ViewDocument>().
                    Where(x => x.Status == "Опубликован").
                    Get();
            foreach (var document in documents.Models)
            {
                FilteredDocuments.Add(document);
            }
        }
        public async void Download()
        {
            var url = await client.Storage.From("documents").CreateSignedUrl(SelectedDocument.Url, 60);
            var dialog = new SaveFileDialog
            {
                FileName = Path.GetFileName(SelectedDocument.Url),
                Filter = "Все файлы|*.*"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetStreamAsync(url);
                using (var fileStream = File.Create(dialog.FileName))
                {
                    await response.CopyToAsync(fileStream);
                }
            }
            MessageBox.Show("Файл сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private async void Search()
        {
            FilteredDocuments.Clear();
            var response = await client.From<ViewDocument>()
                .Where(x => x.Status == "Опубликован")
                .Filter(x => x.Title, Operator.ILike, $"%{SearchQuery}%")
                .Get();
            foreach(var doc in response.Models)
            {
                FilteredDocuments.Add(doc);
            }
        }     
    }
}

