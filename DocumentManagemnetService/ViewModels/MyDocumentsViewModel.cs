using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Category = DocumentManagementService.Models.Category;

namespace DocumentManagementService.ViewModels
{
    public class MyDocumentsViewModel : BaseViewModel
    {
        private readonly Client client;
        private readonly INavigationService navigationService;
        private ObservableCollection<ViewDocument> Documents { get; } = [];
        public ObservableCollection<Category> Categories { get; } = [];
        public ICollectionView FilteredDocuments { get; }
        public User CurrentUser;
        public ICommand SelectionCommand { get; }

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
            get{ return fromDate; }
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
        public MyDocumentsViewModel()
        {
            navigationService = App.NavigationService;
            client = App.SupabaseService.Client;

            SelectionCommand = new RelayCommand(Selection, obj => SelectedDocument != null);

            LoadDocuments();
            LoadCategories();

            FilteredDocuments = new CollectionViewSource 
            {
                Source = Documents

            }.View;

            FilteredDocuments.Filter = obj => FilterDocument(obj as ViewDocument);

        }
        private void ApplyFilters() => FilteredDocuments.Refresh();
        
        private void Selection()
        {
            navigationService.Navigate("Viewer");
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
        private async void LoadDocuments()
        {
            Documents.Clear();
            var documents = await client.From<ViewDocument>()
                .Where(x => x.AuthorId == App.CurrentUser.Id.ToString())
                .Get();
            foreach (var document in documents.Models)
            {
                Documents.Add(document);
            }
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
                !Categories.Where(c => c.IsChecked).Any(c => c.Name == doc.Category) )

                    return false;

            if (FromDate.HasValue && doc.CreatedAt < FromDate.Value)
                return false;
            if (ToDate.HasValue && doc.CreatedAt > ToDate.Value)
                return false;

            return true;
        }
    }
}
