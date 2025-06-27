using DocumentManagementService.Models;
using System.Collections.ObjectModel;

namespace DocumentManagementService.ViewModels
{
    public class DocumentsViewModel: BaseViewModel
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        private readonly INavigationService navigationService;

        private MenuItemViewModel selectedMenuItem;
        public MenuItemViewModel SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set
            {
                if (selectedMenuItem != value)
                {

                    selectedMenuItem = value;
                    OnPropertyChanged();

                    navigationService.Navigate(selectedMenuItem?.PageKey); //Когда пользователь выбирает пункт меню, вызывается переход
                }
            }
        }
        public DocumentsViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            MenuItems =
            [
                new("Новый документ", "PencilPlus", "UploadDocument"),
                new("Все документы", "File", "AllDocuments"),
                new("Входящие", "ArrowBottomLeftThin", "Incoming"),
            ];
            navigationService.Navigate("AllDocuments");
        }
    }
}
