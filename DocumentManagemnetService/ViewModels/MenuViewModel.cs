using DocumentManagementService.Models;
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
        public MenuViewModel(INavigationService navigationService)
        {
            authService = new AuthService(App.SupabaseService.Client);
            LeaveCommand = new RelayCommand(Leave);
            AccountCommand = new RelayCommand(Account);
            this.navigationService = navigationService;
            MenuItems =
            [
                new("Новый документ", "PencilPlus", "UploadDocument"),
                new("Все документы", "File", "AllDocuments"),
                new("Мои документы", "AccountFileTextOutline", "MyDocuments"),
                new("Входящие", "ArrowRightThin", "Incoming"),
                new("Исходящие", "ArrowLeftThin", "Outcoming"),
            ];
            navigationService.Navigate("AllDocuments");
        }
        //public MenuViewModel(INavigationService navigationService)
        //{
        //    LeaveCommand = new RelayCommand(Leave);
        //    authService = new AuthService(App.SupabaseService.Client);
        //    this.navigationService = navigationService;
        //    MenuItems =
        //    [
        //        new("Документы", "FileDocument", "Documents"),
        //    ];
        //    navigationService.Navigate("Documents");
        //}
        private void Account()
        {
            navigationService.Navigate("Account");
        }
        private async void Leave()
        {
            await authService.SignOutAsync();
            Application.Current.MainWindow = new LoginView();
            Application.Current.MainWindow.Show();
            CloseAction();
        }
    }
}
