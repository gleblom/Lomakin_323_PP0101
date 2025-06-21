using DocumentManagementService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.ViewModels
{
    public class DocumentsViewModel: BaseViewModel
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
        public DocumentsViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.navigationService = navigationService;
            MenuItems =
            [
                new("Новый документ", "PencilPlus", "UploadDocument"),
                new("Все документы", "File", "AllDocuments"),
                new("Входящие", "ArrowBottomLeftThin", "Incoming"),
                new("Исходящие", "ArrowTopRightThin", "Outcoming"),
                new("Черновики", "PencilOutline", "Draft")

            ];
        }
    }
}
