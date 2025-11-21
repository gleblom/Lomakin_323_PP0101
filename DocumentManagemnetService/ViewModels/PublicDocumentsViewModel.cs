using DocumentManagementService.Models;
using DocumentManagemnetService;
using Microsoft.Win32;
using Supabase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Category = DocumentManagementService.Models.Category;


namespace DocumentManagementService.ViewModels
{
    public class PublicDocumentsViewModel : BaseViewModel
    {
        private readonly Client client;
        private readonly INavigationService navigationService;
        private readonly DocumentService documentService;
        public string SearchQuery { get; set; }
        private ObservableCollection<ViewDocument> Documents { get; } = [];
        public ObservableCollection<Category> Categories { get; } = [];
        public ObservableCollection<User> Users { get; } = [];
        public ICollectionView FilteredDocuments { get; }
        public ICommand SelectionCommand { get; }
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
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }
        private DateTime? fromDate;
        public DateTime? FromDate
        {
            get { return fromDate; }
            set
            {
                fromDate = value;
                OnPropertyChanged();
                ApplyFilters();

            }
        }
        private DateTime? toDate;
        public DateTime? ToDate
        {
            get { return toDate; }
            set
            {
                toDate = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        private void ApplyFilters() => FilteredDocuments.Refresh();
        public PublicDocumentsViewModel(string key)
        {
            navigationService = App.NavigationService;
            client = App.SupabaseService.Client;
            documentService = new();

            SelectionCommand = new RelayCommand(Selection, obj => SelectedDocument != null);
            DownloadCommand = new RelayCommand(Download, obj => SelectedDocument != null);

            if (key == "AllDocuments")
            {
                LoadDocuments();
            }
            else
            {
                LoadNotifications();
            }
            LoadUsers();
            LoadCategories();


            FilteredDocuments = new CollectionViewSource
            {
                Source = Documents

            }.View;

            FilteredDocuments.Filter = obj => FilterDocument(obj as ViewDocument);
        }

        private void Selection()
        {
            navigationService.Navigate("Viewer");
        }
        private async void LoadDocuments()
        {
            Documents.Clear();
            var documents = await client.From<ViewDocument>().
                    Where(x => x.Status == "Опубликован").
                    Get();
            foreach (var document in documents.Models)
            {
                Documents.Add(document);
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
            using (HttpClient httpClient = new())
            {
                var response = await httpClient.GetStreamAsync(url);
                using (var fileStream = File.Create(dialog.FileName))
                {
                    await response.CopyToAsync(fileStream);
                }
            }
            MessageBox.Show("Файл сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private bool FilterDocument(ViewDocument doc)
        {
            if (Categories.All(c => c.IsChecked == false))
                return false;


            if (doc == null)
                return false;


            if (!string.IsNullOrWhiteSpace(Title) &&
                !doc.Title.Contains(Title, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Categories.Any(c => c.IsChecked) &&
                !Categories.Where(c => c.IsChecked).Any(c => c.Name == doc.Category))

                return false;

            if (FromDate.HasValue && doc.CreatedAt < FromDate.Value)
                return false;
            if (ToDate.HasValue && doc.CreatedAt > ToDate.Value)
                return false;

            return true;
        }
        private async void LoadUsers()
        {
            var users = await client.From<User>()
                 .Where(x => x.RoleId != 1 || x.RoleId != 2)
                 .Where(x => x.UnitId == App.CurrentUser.UnitId)
                 .Get();
            foreach (var user in users.Models)
            {
                user.IsChecked = true;
                Users.Add(user);
            }
        }
        public async void LoadCategories()
        {
            Categories.Clear();
            var categories = await client.From<Category>().Get();
            foreach (var category in categories.Models)
            {
                category.IsChecked = true;
                Categories.Add(category);
            }
            foreach (var item in Categories)
            {
                item.PropertyChanged += (s, e) => ApplyFilters();
            }
            ApplyFilters();
        }
        private async void LoadNotifications()
        {
            List<Notification> notifications = await documentService.GetNotificationsAsync();
            Documents.Clear();
            foreach (var notification in notifications)
            {
                var document = await client.From<ViewDocument>().
                    Where(x => x.Id == notification.DocumentId).
                    Get();
                Documents.Add(document.Model);
            }
            ApplyFilters();
        }

    }
}

