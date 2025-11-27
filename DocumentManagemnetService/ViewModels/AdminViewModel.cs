using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using NLog;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
            client = App.SupabaseService.Client;
            CurrentUser = App.CurrentUser;

            EditUserCommand = new RelayCommand(OpenUserEditor, 
                obj => !App.IsWindowOpen<UserEditView>() && SelectedUser != null);
            AddUserCommand = new RelayCommand(AddUser, 
                obj => !App.IsWindowOpen<UserEditView>());

            LoadUsers();
            LoadRoles();
            LoadUnits();

            FilteredUsers = new CollectionViewSource
            {
                Source = Users
            }.View;


            FilteredUsers.Filter = obj => FilterUsers(obj as UserView);
        }

        private void ApplyFilters() => FilteredUsers.Refresh();
        private void AddUser()
        {
            App.SelectedUser = null;
            OpenUserEditor();
        }
        private void OpenUserEditor()
        {
            try
            {
                var userEditWindow = new UserEditView();
                UserEditViewModel vm = new();
                vm.UpdateAction ??= new Action(LoadUsers);
                userEditWindow.DataContext = vm;
                userEditWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }
        private async void LoadRoles()
        {
            Roles.Clear();
            var roles = await client.From<Role>()
                .Where(x => x.Id != 1)
                .Where(x => x.Id != 2)
                .Where(x => x.Id != 3)
                .Get();
            foreach(var role in roles.Models)
            {
                Roles.Add(role);
            }
            foreach (var item in Roles)
            {
                item.PropertyChanged += (s, e) => ApplyFilters();
            }
            ApplyFilters();
        }
        private async void LoadUnits()
        {
            Units.Clear();
            var units = await client.From<Unit>().Get();
            foreach(var unit in units.Models)
            {
                Units.Add(unit);
            }
            foreach (var item in Units)
            {
                item.PropertyChanged += (s, e) => ApplyFilters();
            }
            ApplyFilters();
        }
        private async void LoadUsers()
        {
            Users.Clear();
            var users = await client.From<UserView>().
                Where(x => x.CompanyId == App.CurrentUser.CompanyId)
                .Where(x => x.UnitId != 0)
                .Get();

            foreach (var user in users.Models)
            {
                Users.Add(user);
            }
        }

        private bool FilterUsers(UserView user)
        {
            if(user == null)
            {
                return false;
            }
            if (Units.All(x => x.IsChecked == false) || Roles.All(x => x.IsChecked == false))
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
    }
}
