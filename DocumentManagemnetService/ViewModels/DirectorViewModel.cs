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
    public class DirectorViewModel: BaseViewModel
    {
        private readonly Client client;
        public ObservableCollection<ViewRole> Roles { get; } = [];
        private ObservableCollection<User> Users { get; } = [];
        public ObservableCollection<Unit> Units { get; } = [];
        public ICollectionView FilteredUsers { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public User CurrentUser { get; }
        private User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                if (selectedUser != value)
                {
                    selectedUser = value;
                    OnPropertyChanged();


                    App.SelectedExecutive = SelectedUser;


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
        public DirectorViewModel()
        {
            client = App.SupabaseService.Client;
            CurrentUser = App.CurrentUser;

            EditUserCommand = new RelayCommand(OpenUserEditor,
    obj => !App.IsWindowOpen<ClerkEditView>() && SelectedUser != null);
            AddUserCommand = new RelayCommand(AddUser,
                obj => !App.IsWindowOpen<ClerkEditView>());

            LoadUsers();


            FilteredUsers = new CollectionViewSource
            {
                Source = Users
            }.View;


            FilteredUsers.Filter = obj => FilterUsers(obj as User);
        }
        private void ApplyFilters() => FilteredUsers.Refresh();
        private void AddUser()
        {
            App.SelectedUser = null;
            OpenUserEditor();
        }
        private void OpenUserEditor()
        {
            var userEditWindow = new ClerkEditView();
            ClerkEditViewModel vm = new();
            vm.UpdateAction ??= new Action(LoadUsers);
            userEditWindow.DataContext = vm;
            userEditWindow.ShowDialog();

        }
        private void SyncRoles()
        {
            foreach (var user  in Users)
            {
                var role = Roles.Where(x => x.Id == user.RoleId).Single();
                user.Role = role;
            }
            ApplyFilters();
        }
        private async void LoadUsers()
        {
            Roles.Clear();
            var roles = await client.From<ViewRole>()
                .Where(x => x.Id == 1 || x.Id == 2)
                .Get();
            foreach (var role in roles.Models)
            {
                Roles.Add(role);
            }
            foreach (var item in Roles)
            {
                item.PropertyChanged += (s, e) => ApplyFilters();
            }

            var users = await client.From<User>()
                .Where(x => x.RoleId == 2 || x.RoleId == 1)
                .Get();

            foreach (var user in users.Models)
            {
                Users.Add(user);
            }
            SyncRoles();
            ApplyFilters();
        }


        private bool FilterUsers(User user)
        {
            if (user == null)
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
