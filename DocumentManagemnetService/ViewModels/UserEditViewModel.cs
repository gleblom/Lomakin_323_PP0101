using DocumentManagementService.Models;
using DocumentManagemnetService;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Client = Supabase.Client;
using User = DocumentManagementService.Models.User;

namespace DocumentManagementService.ViewModels
{
    public class UserEditViewModel: BaseViewModel
    {
        private readonly Client client;
        private readonly INavigationService navigationService;
        public ObservableCollection<Unit> Units { get; } = [];
        public ObservableCollection<Role> Roles { get; } = [];
        public ObservableCollection<Category> Categories { get; } = [];
        public ICommand AddUnitCommand { get; }
        public ICommand AddRoleCommand  { get; }
        public ICommand BackCommand { get; }
        public ICommand EditRoleCommand { get; }
        public ICommand EditUnitCommand { get; }
        public ICommand SaveUserCommand { get; }
        public ICommand SaveRoleCommand { get; }
        public ICommand SaveUnitCommand { get; }
        public ICommand RemoveUnitCommand { get; }
        public ICommand RemoveRoleCommand { get; }
        public ICommand RemoveUserCommand { get; }

        private string role;
        public string Role
        {
            get { return role; }
            set 
            { 
                role = value;
                OnPropertyChanged();
            }
        }
        private bool isEditing;

        private Visibility isAddRoleVisible = Visibility.Hidden;
        public Visibility IsAddRoleVisible
        {
            get { return isAddRoleVisible; }
            set
            {
                isAddRoleVisible = value;
                OnPropertyChanged();
            }
        }
        private Visibility isAddUnitVisible = Visibility.Hidden;
        public Visibility IsAddUnitVisible
        {
            get { return isAddUnitVisible; }
            set
            {
                isAddUnitVisible = value; 
                OnPropertyChanged();
            }
        }

        private User currentUser;
        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnPropertyChanged();
            }
        }


        private Role selectedRole;
        public Role SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                OnPropertyChanged();
            }
        }
        private Unit selectedUnit;
        public Unit SelectedUnit
        {
            get { return selectedUnit; }
            set
            {

                selectedUnit = value;
                OnPropertyChanged();

            }
        }
        public UserEditViewModel() 
        {
            navigationService = App.NavigationService;
            client = App.SupabaseService.Client;
            CurrentUser = App.SelectedUser;

            CurrentUser.RoleId = App.SelectedUser.RoleId;
            CurrentUser.UnitId = App.SelectedUser.UnitId;
            CurrentUser.CompanyId = App.SelectedUser.CompanyId;

            LoadUnits();
            LoadRoles();
            LoadCategories();

            AddUnitCommand = new RelayCommand(AddUnit);
            AddRoleCommand = new RelayCommand(AddRole);
            BackCommand = new RelayCommand(Back);
            EditRoleCommand = new RelayCommand(EditRole);

        }

        private void EditRole()
        {
            IsAddRoleVisible = Visibility.Visible;
            isEditing = true;
            Role = SelectedRole.Name;
            CheckCategory();
        }

        private void CheckCategory()
        {
            for (int i = 0; i < SelectedRole.Categories.Count; i++)
            {
                if (Categories[i].Id == SelectedRole.Categories[i].Id)
                {
                    Categories[i].IsChecked = true;
                }
            }
        }
        private void Back()
        {
            navigationService.Navigate("AdminView");
        }

        private void AddUnit()
        {
            IsAddUnitVisible = Visibility.Visible;
        }

        private void AddRole()
        {
            IsAddRoleVisible = Visibility.Visible;
        }

        private async void LoadUnits()
        {
            Units.Clear();
            var units = await client.From<Unit>().Where(x => x.CompanyId == CurrentUser.CompanyId).Get();
            foreach (var unit in units.Models)
            {
                Units.Add(unit);
                if(unit.Id == CurrentUser.UnitId)
                {
                    SelectedUnit = unit;
                }
            }
        }

        private async void LoadRoles()
        {
            Roles.Clear();
            var roles = await client.From<Role>().Get();
            foreach (var role in roles.Models)
            {
                Roles.Add(role);
                if (role.Id == CurrentUser.RoleId)
                {
                    SelectedRole = role;
                }
            }
        }
        private async void LoadCategories()
        {
            Categories.Clear();
            var categories = await client.From<Category>().Get();
            foreach(var category in categories.Models)
            {
                category.IsChecked = false;
                Categories.Add(category);
            }
        }
       
    }
}
