using DocumentManagementService.Models;
using System.Collections.ObjectModel;

namespace DocumentManagementService.ViewModels
{
    class AccountViewModel: BaseViewModel
    {
        private readonly INavigationService navigationService;
        public ObservableCollection<MenuItemModel> Items { get; }

        private MenuItemModel selectedMenuItem;
        public MenuItemModel SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set
            {
                if (selectedMenuItem != value)
                {

                    selectedMenuItem = value;
                    OnPropertyChanged();

                    navigationService.Navigate(selectedMenuItem?.PageKey); 
                }
            }
        }
        public AccountViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            Items =
                [
                    new("Профиль", "Account", "Profile")
                ];
        }
    }
}
