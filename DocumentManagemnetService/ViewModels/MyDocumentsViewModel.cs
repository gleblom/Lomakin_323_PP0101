
using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class MyDocumentsViewModel: BaseViewModel
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
        public MyDocumentsViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            client = App.SupabaseService.Client;
            LoadDocuments();
        }
        private async void LoadDocuments()
        {
            FilteredDocuments.Clear();
            var documents = await client.From<ViewDocument>().
                Where(x => x.AuthorId == client.Auth.CurrentUser.Id).
                Get();
            foreach(var document in documents.Models)
            {
                FilteredDocuments.Add(document);
            }
        }
    }
}
