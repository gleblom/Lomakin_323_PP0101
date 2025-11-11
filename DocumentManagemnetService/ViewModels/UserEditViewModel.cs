using DocumentManagementService.Models;
using DocumentManagemnetService;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Client = Supabase.Client;
using User = DocumentManagementService.Models.User;

namespace DocumentManagementService.ViewModels
{
    public class UserEditViewModel: BaseViewModel
    {
        private readonly Client client;
        private readonly AuthService authService;
        public ObservableCollection<Unit> Units { get; } = [];
        public ObservableCollection<Role> Roles { get; } = [];
        public ObservableCollection<Category> Categories { get; } = [];
        public ICommand AddUnitCommand { get; }
        public ICommand AddRoleCommand  { get; }
        public ICommand EditRoleCommand { get; }
        public ICommand EditUnitCommand { get; }
        public ICommand SaveUserCommand { get; }
        public ICommand SaveRoleCommand { get; }
        public ICommand SaveUnitCommand { get; }

        private string password;
        public string Password
        {
            get { return password; }
            set 
            {
                password = value;
                OnPropertyChanged();
            }
        }

        private string role;
        public string CurrentRole
        {
            get { return role; }
            set 
            { 
                role = value;
                OnPropertyChanged();
            }
        }
        private string unit;
        public string CurrentUnit
        {
            get { return unit; }
            set
            {            
                unit = value; 
                OnPropertyChanged(); 
            }
        }

        private bool isUserEditing;
        private bool isRoleEditing;
        private bool isUnitEditing;
        private bool isReadOnly;
        public bool IsReadOnly
        {
            get { return  isReadOnly; }
            set
            {
                isReadOnly = value;
                OnPropertyChanged();
            }
        }

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

        private UserView currentUser;
        public UserView CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnPropertyChanged();
            }
        }


        private Role selectedRole;
        public Role? SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                OnPropertyChanged();
            }
        }
        private Unit selectedUnit;
        public Unit? SelectedUnit
        {
            get { return selectedUnit; }
            set
            {

                selectedUnit = value;
                if (isUnitEditing)
                {
                    CurrentUnit = SelectedUnit.Name;
                }
                OnPropertyChanged();

            }
        }
        public UserEditViewModel() 
        {
            client = App.SupabaseService.Client;

            if (App.SelectedUser != null)
            {
                CurrentUser = App.SelectedUser;
                authService = new AuthService();

                CurrentUser.Id = App.SelectedUser.Id;
                CurrentUser.RoleId = App.SelectedUser.RoleId;
                CurrentUser.UnitId = App.SelectedUser.UnitId;
                CurrentUser.CompanyId = App.SelectedUser.CompanyId;

                isUserEditing = true;
                isReadOnly = true;
            }
            else
            {
                isReadOnly = false;
                isUserEditing = false;
                CurrentUser = new();
            }

            LoadUnits();
            LoadRoles();
            LoadCategories();

            AddUnitCommand = new RelayCommand(AddUnit);
            AddRoleCommand = new RelayCommand(AddRole);
            EditRoleCommand = new RelayCommand(EditRole, obj => SelectedRole != null);
            EditUnitCommand = new RelayCommand(EditUnit, obj => SelectedUnit != null);
            SaveRoleCommand = new RelayCommand(SaveRole, obj => CurrentRole != String.Empty && Categories.Any(x => x.IsChecked));
            SaveUnitCommand = new RelayCommand(SaveUnit, obj => CurrentUnit != String.Empty);

            SaveUserCommand = new RelayCommand(SaveUser, obj =>

                CurrentUser.FirstName != String.Empty && CurrentUser.SecondName != String.Empty && CurrentUser.Email != String.Empty &&
                CurrentUser.Telephone != String.Empty && SelectedRole != null || SelectedUnit != null);


        }
        public async Task ChangeUserPassword(string adminToken, string userId, string newPassword)
        {
            using var http = new HttpClient();
            var json = JsonSerializer.Serialize(new {id = userId, new_password = newPassword });

            var req = new HttpRequestMessage(HttpMethod.Post, "https://kphkeykctqyqgfotrqoy.supabase.co/functions/v1/change_password");
            req.Headers.Add("Authorization", $"Bearer {adminToken}");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();
            if(res.StatusCode == HttpStatusCode.OK)
            {
                MessageBox.Show("Пароль успешно изменен!", "Изменение пароля", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void SaveUnit()
        {

            if (isUnitEditing)
            {
                var update = await client.From<Unit>()
                    .Where(x => x.Id == SelectedRole.Id)
                    .Set(x => x.Name, CurrentUnit)
                    .Update();
                if(update.Models.Count == 1)
                {
                    MessageBox.Show("Отдел успешно обновлен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                    CurrentRole = String.Empty;
                    LoadUnits();
                }
                else
                {
                    MessageBox.Show("При сохранении отдела произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                var unit = new Unit
                {
                    Name = CurrentUnit,
                    CompanyId = CurrentUser.CompanyId
                };  
                var respone =  await client.From<Unit>().Insert(unit);
                if (respone.Models.Count == 1)
                {
                    MessageBox.Show("Новый отдел успешно добавлен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                    CurrentRole = String.Empty;
                    LoadUnits();
                }
                else
                {
                    MessageBox.Show("При сохранении отдела произошла ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async void SaveUser()
        {
            if (isUserEditing)
            {

                string id = CurrentUser.Id.ToString();
                if (password != String.Empty)
                {
                    await ChangeUserPassword(client.Auth.CurrentSession.AccessToken, id, password);
                }

                await client.From<User>()
                     .Where(x => x.Id == currentUser.Id)
                     .Set(x => x.FirstName, CurrentUser.FirstName)
                     .Set(x => x.SecondName, CurrentUser.SecondName)
                     .Set(x => x.ThirdName, CurrentUser.ThirdName)
                     .Set(x => x.Email, CurrentUser.Email)
                     .Set(x => x.UnitId, SelectedUnit.Id)
                     .Set(x => x.RoleId, SelectedRole.Id)
                     .Update();
            }
            else
            {
                await authService.SignUpAsync(CurrentUser.Email, Password,
                     CurrentUser.FirstName, CurrentUser.SecondName, CurrentUser.ThirdName,
                        CurrentUser.Telephone, SelectedRole.Id, SelectedUnit.Id);
                CurrentUser = null;
            }
        }
        private async void AddRoleCategory(List<RoleCategory> roleCategory, Role role)
        {
            roleCategory.Clear();
            foreach (var category in Categories)
            {
                if (category.IsChecked)
                {
                    roleCategory.Add(
                        new RoleCategory
                        {
                            CategoryId = category.Id,
                            Category = category,
                            CompanyId = CurrentUser.CompanyId,
                            Role = role,
                            RoleId = role.Id
                        }
                    );
                }
            }
            await client.From<RoleCategory>().Insert(roleCategory);
        }
        private async void SaveRole()
        {

            if (isRoleEditing)
            {
                var currentRoleCategories = await client
                    .From<RoleCategory>()
                    .Where(rc => rc.RoleId == SelectedRole.Id)
                    .Get();

                var selectedCategoryIds = Categories.Where(x => x.IsChecked == true).Select(c => c.Id).ToHashSet();


                var currentCategoryIds = currentRoleCategories.Models
                    .Select(rc => rc.CategoryId)
                    .ToHashSet();

                var categoriesToAdd = selectedCategoryIds.Except(currentCategoryIds);
                var categoriesToRemove = currentCategoryIds.Except(selectedCategoryIds);

                var newRoleCategories = categoriesToAdd.Select(categoryId => new RoleCategory
                {
                    RoleId = SelectedRole.Id,
                    CategoryId = categoryId,
                    CompanyId = CurrentUser.CompanyId
                }).ToList();

                if (newRoleCategories.Any())
                {
                    await client.From<RoleCategory>().Insert(newRoleCategories);
                }
                if (categoriesToRemove.Any())
                {


                    var recordsToDelete = currentRoleCategories.Models
                        .Where(rc => categoriesToRemove.Contains(rc.CategoryId))
                        .ToList();
                    foreach (var record in recordsToDelete)
                    {
                        await client.From<RoleCategory>()
                            .Where(rc => rc.Id == record.Id)
                            .Delete();
                    }
                }

                await client.From<Role>()
                    .Where(x => x.Id == SelectedRole.Id)
                    .Set(x => x.Name, CurrentRole)
                    .Update();

                CurrentRole = String.Empty;
                UnCheckCategories();

                MessageBox.Show("Изменения успешно сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {

                var role = new Role
                {
                    Id = Roles.Count + 4,
                    Name = CurrentRole
                };
                var response = await client.From<Role>().Insert(role);

                if(response.Models.Count == 1)
                {
                    var roleCategory = new List<RoleCategory>();
                    AddRoleCategory(roleCategory, response.Model);
                    CurrentRole = String.Empty;
                    UnCheckCategories();
                    LoadRoles();
                    MessageBox.Show("Должность успешно сохранена!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void UnCheckCategories()
        {
            foreach(var category in Categories)
            {
                category.IsChecked = false;
            }
        }

        private void EditUnit()
        {
            IsAddUnitVisible = Visibility.Visible;
            CurrentUnit = SelectedUnit.Name;
            isUnitEditing = true;
        }
        private void EditRole()
        {
            IsAddRoleVisible = Visibility.Visible;
            isRoleEditing = true;
            CurrentRole = SelectedRole.Name;
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
        private void AddUnit()
        {
            IsAddUnitVisible = Visibility.Visible;
            CurrentUnit = String.Empty;
        }

        private void AddRole()
        {
            IsAddRoleVisible = Visibility.Visible;
            CurrentRole = String.Empty;
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
