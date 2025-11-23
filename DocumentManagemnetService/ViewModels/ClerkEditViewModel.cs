using DocumentManagementService.Models;
using DocumentManagemnetService;
using NLog;
using Supabase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class ClerkEditViewModel: BaseViewModel
    {
        private readonly Client client;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AuthService authService;
        private readonly bool isUserEditing;
        public ICommand SaveUserCommand { get; }
        public Action UpdateAction { get; set; }
        public ObservableCollection<ViewRole> Roles { get; } = [];
        private bool isReadOnly;
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
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                OnPropertyChanged();
            }
        }
        private ViewRole selectedRole;

        public ViewRole? SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                OnPropertyChanged();
            }
        }



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
        public ClerkEditViewModel()
        {
            client = App.SupabaseService.Client;
            authService = new AuthService(App.SupabaseService);

            if (App.SelectedUser != null)
            {
                CurrentUser = App.SelectedExecutive;

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
            SaveUserCommand = new RelayCommand(SaveUser, obj =>
            CurrentUser.FirstName != null && CurrentUser.SecondName != null &&
            CurrentUser.Email != null && SelectedRole != null && Password != null &&
            CurrentUser.Telephone != null && CurrentUser.Telephone?.Length > 9 && Password?.Length > 5);

            LoadAdminsClerks();
        }
        private async void SaveUser()
        {
            try
            {
                if (isUserEditing)
                {

                    string id = CurrentUser.Id.ToString();
                    if (password != string.Empty)
                    {
                        await AuthService.ChangeUserPassword(id, password);
                    }

                    await client.From<User>()
                         .Where(x => x.Id == currentUser.Id)
                         .Set(x => x.FirstName, CurrentUser.FirstName)
                         .Set(x => x.SecondName, CurrentUser.SecondName)
                         .Set(x => x.ThirdName, CurrentUser.ThirdName)
                         .Set(x => x.RoleId, SelectedRole.Id)
                         .Update();
                }
                else
                {
                    string token = client.Auth.CurrentSession.AccessToken;
                    bool success = await authService.SignUpAsync(CurrentUser.Email, Password,
                          CurrentUser.FirstName, CurrentUser.SecondName, CurrentUser.ThirdName,
                             CurrentUser.Telephone, SelectedRole.Id, null, App.CurrentUser.CompanyId);
                    if (success)
                    {
                        MessageBox.Show("Новый пользователь успешно создан!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);

                        string id = App.RegisteredUser.Id.ToString();

                        await AuthService.SendEmail(token, id, password);
                        CurrentUser = new();
                        Password = string.Empty;
                        SelectedRole = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            UpdateAction();
            
        }
        private async void LoadAdminsClerks()
        {
            Roles.Clear();
            var roles = await client.From<ViewRole>()
                .Where(x => x.Id == 1 || x.Id ==2)
                .Get();
            foreach (var role in roles.Models)
            {
                Roles.Add(role);
                if (role.Id == CurrentUser.RoleId)
                {
                    SelectedRole = role;
                }
            }
        }      
    }
}
