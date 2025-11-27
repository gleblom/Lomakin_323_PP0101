using DocumentManagementService.Models;
using DocumentManagemnetService;
using System.Collections.ObjectModel;


namespace DocumentManagementService.ViewModels
{
    class ViewerViewModel: BaseViewModel
    {
        private readonly INavigationService navigationService;
        public ObservableCollection<MenuItemModel> DocumentItems { get; }

        private MenuItemModel selectedDocumentItem;
        public MenuItemModel SelectedDocumentItem
        {
            get { return selectedDocumentItem; }
            set
            {
                if (selectedDocumentItem != value)
                {

                    selectedDocumentItem = value;
                    OnPropertyChanged();

                    navigationService.Navigate(selectedDocumentItem?.PageKey); 
                }
            }
        }
        public ViewerViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            DocumentItems =
                [
                    new("Просмотр", "Image", "View"),
                ];
            if(App.SelectedDocument.RouteId != null)
            {
                DocumentItems.Add(new("Согласование", "Check", "Approvement"));
            }

        }
    }
}
