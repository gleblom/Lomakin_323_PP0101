using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using Supabase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly Client client;
        private readonly INavigationService navigationService;
        private ObservableCollection<UserView> Users { get; } = [];
        public ObservableCollection<Unit> Units { get; } = [];
        public ObservableCollection<Role> Roles { get; } = [];
        public ICollectionView FilteredUsers { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public User CurrentUser { get; }
        private UserView selectedUser;
        public UserView SelectedUser
        {
            get { return selectedUser; }
            set
            {
                if (selectedUser != value)
                {
                    selectedUser = value;
                    OnPropertyChanged();


                    App.SelectedUser = SelectedUser;

                    
                    //navigationService.Navigate("UserEdit");
                }
            }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }
        public AdminViewModel()
        {
            navigationService = App.NavigationService;
            client = App.SupabaseService.Client;
            CurrentUser = App.CurrentUser;

            EditUserCommand = new RelayCommand(OpenUserEditor, 
                obj => !App.IsWindowOpen<UserEditView>() && SelectedUser != null);

            LoadUsers();
            LoadRoles();
            LoadUnits();

            FilteredUsers = new CollectionViewSource
            {
                Source = Users
            }.View;


            FilteredUsers.Filter = obj => FilterUsers(obj as User);
        }

        private void ApplyFilters() => FilteredUsers.Refresh();
        private void OpenUserEditor()
        {
            var userEditWindow = new UserEditView();
            UserEditViewModel vm = new();
            userEditWindow.DataContext = vm;
            userEditWindow.ShowDialog();

        }
        private async void LoadRoles()
        {
            Roles.Clear();
            var roles = await client.From<Role>().Get();
            foreach(var role in roles.Models)
            {
                Roles.Add(role);
            }
            foreach (var item in Roles)
            {
                item.PropertyChanged += (s, e) => ApplyFilters();
            }
        }
        private async void LoadUnits()
        {
            Units.Clear();
            var units = await client.From<Unit>().Where(x => x.CompanyId == CurrentUser.CompanyId).Get();
            foreach(var unit in units.Models)
            {
                Units.Add(unit);
            }
            foreach (var item in Units)
            {
                item.PropertyChanged += (s, e) => ApplyFilters();
            }
        }
        private async void LoadUsers()
        {
            var users = await client.From<UserView>().
                Where(x => x.CompanyId == App.CurrentUser.CompanyId).
                Get();

            foreach (var user in users.Models)
            {
                Users.Add(user);
            }
        }

        private bool FilterUsers(User user)
        {
            if(user == null)
            {
                return false;
            }
            if (user.RoleId == 1 || user.RoleId == 2 || user.RoleId == 3)
            {
                return false;
            }
            if (Units.Any(x => x.IsChecked)
                && !Units.Where(x => x.IsChecked).Any(x => x.Id == user.UnitId)) 
            {
                return false;
            }
            if (Roles.Any(x => x.IsChecked)
                && !Roles.Where(x => x.IsChecked).Any(x => x.Id == user.RoleId)) 
            { 
                return false; 
            }
            if (!string.IsNullOrWhiteSpace(Name) &&
                !user.Display.Contains(Name, StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        } 
        private void AddUser()
        {
            navigationService.Navigate("UserEdit");
        }
    }
}
