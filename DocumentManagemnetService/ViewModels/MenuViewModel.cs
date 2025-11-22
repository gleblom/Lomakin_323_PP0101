using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using DocumentManagemnetService.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class MenuViewModel: BaseViewModel
    {
        public ObservableCollection<MenuItemModel> MenuItems { get; }
        public ICommand LeaveCommand { get; }
        public ICommand AccountCommand { get; }
        public Action CloseAction { get; set; } //Делегат для закрытия данного окна (как в LoginViewModel)

        private readonly INavigationService navigationService; 

        private MenuItemModel selectedMenuItem;
        private readonly AuthService authService; 
        
        public MenuItemModel SelectedMenuItem 
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
        public MenuViewModel(int? userRole)
        {
            authService = new AuthService();
            LeaveCommand = new RelayCommand(Leave, obj => !App.IsWindowOpen<StartupView>());
            AccountCommand = new RelayCommand(Account);
            navigationService = App.NavigationService;
            switch (userRole)
            {
                case 1:
                    navigationService.Navigate("AdminView");
                    break;
                case 2:
                    navigationService.Navigate("ClerkView");
                    break;
                case 3:
                    MenuItems =
                    [
                        new("Новый документ", "PencilPlus", "UploadDocument"),
                        new("Все документы", "File", "AllDocuments"),
                        new("Мои документы", "AccountFileTextOutline", "MyDocuments"),
                        new("Входящие", "ArrowRightThin", "Incoming"),
                        new("Дирекция", "AccountArrowUpOutline", "Director")
                   ];
                   
                    navigationService.Navigate("AllDocuments");
                    break;
                default:
                    MenuItems =
                    [
                        new("Новый документ", "PencilPlus", "UploadDocument"),
                        new("Все документы", "File", "AllDocuments"),
                        new("Мои документы", "AccountFileTextOutline", "MyDocuments"),
                        new("Входящие", "ArrowRightThin", "Incoming"),
                   ];
                    navigationService.Navigate("AllDocuments");
                    break;

            }
        }

        private void Account()
        {
            navigationService.Navigate("Account");
        }
        private async void Leave()
        {
            MenuItems.Clear();
            if (!App.IsWindowOpen<StartupView>())
            {
                await authService.SignOutAsync();
                Application.Current.MainWindow = new StartupView();
                Application.Current.MainWindow.Show();
                CloseAction();
            }
        }
    }
}
