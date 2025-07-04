﻿using DocumentManagementService.Models;
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
            LeaveCommand = new RelayCommand(Leave);
            authService = new AuthService(App.SupabaseService.Client);
            this.navigationService = navigationService;
            MenuItems =
            [
                new("Документы", "FileDocument", "Documents"),
                new("Маршруты", "Map", "Routes"),
            ];
            navigationService.Navigate("Documents");
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
