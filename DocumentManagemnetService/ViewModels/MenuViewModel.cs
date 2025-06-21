using DocumentManagementService.Models;
using System;
using System.Collections.ObjectModel;

namespace DocumentManagementService.ViewModels
{
    public class MenuViewModel: BaseViewModel
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        private readonly INavigationService navigationService; 

        private MenuItemViewModel selectedMenuItem;
        public Action<string> NavigateAction { get; set; } //Связь между представлением и модель представления
        public MenuItemViewModel SelectedMenuItem 
        {
            get { return selectedMenuItem; }
            set
            {
                if (selectedMenuItem != value)
                {
                    
                    selectedMenuItem = value;
                    OnPropertyChaneged();

                   navigationService.Navigate(selectedMenuItem?.PageKey); //Когда пользователь выбирает пункт меню, вызывается переход
                }
            } 
        }
        public MenuViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("Документы", "FileDocument", "Documents"),
                new MenuItemViewModel("Маршруты", "Map", "Routes"),
            };
        }
    }
}
