using DocumentManagementService.Models;
using DocumentManagemnetService;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RouteEditorViewModel: BaseViewModel
    {
        private readonly Client client;
        private readonly GraphService graphService;
        private ApprovalRoute? editingRoute;
        public ObservableCollection<RouteStep> Steps { get; } = []; //Список этапов (шагов) для построения графа
        public ObservableCollection<UserView> Users { get; } = [];
        public ICollectionView FilteredUsers { get;  } 
        public ObservableCollection<Unit> Units { get; } = [];
        public RouteStep SelectedStep { get; set;  }
        public User CurrentUser { get; set; }
        public Action UpdateAction { get; set; } //Делегат для обновления списка маршрутов на RoutesView
        public Action UnselectAction { get; set; } //Делегат для обнуления выбранного
        public string RouteName {  get; set; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        private UserView selectedUser;
        public UserView SelectedUser
        { 
            get { return selectedUser; }
            set
            {
                selectedUser = value;
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
                LoadUsers();
                OnPropertyChanged();
            }
        }

        private IBidirectionalGraph<RouteNode, RouteEdge> graph;
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph   //Граф для отображения шагов маршута
        {
            get { return graph ; }
            set
            {
                if (graph != value)
                {
                    graph = value ;
                    OnPropertyChanged();
                }
            }
        }

        private void ApplyFilters() => FilteredUsers.Refresh();
        public RouteEditorViewModel(GraphService graphService, ApprovalRoute? route = null)
        {
            CurrentUser = App.CurrentUser;
            client = App.SupabaseService.Client;
            editingRoute = route;
            this.graphService = graphService;

            AddStepCommand = new RelayCommand(AddStep, obj => SelectedUser != null);
            RemoveStepCommand = new RelayCommand(RemoveStep, obj => SelectedStep != null);
            MoveUpCommand = new RelayCommand(MoveUp, obj => SelectedStep != null);
            MoveDownCommand = new RelayCommand(MoveDown, obj => SelectedStep != null);
            SaveCommand = new RelayCommand(SaveRoute, obj => (RouteName != null && Steps.Count > 1));
            DeleteCommand = new RelayCommand(DeleteRoute, obj => editingRoute != null);

            //LoadUsers();
            LoadUnits();


            FilteredUsers = new CollectionViewSource
            {
                Source = Users

            }.View;

            FilteredUsers.Filter = obj => FilterUsers(obj as UserView);

            if (editingRoute != null)
            {
                //LoadRoute(editingRoute.GraphJson);
               Graph = graphService.LoadRoute(editingRoute.GraphJson, Steps);
               graphService.UpdateRoute(editingRoute, Steps);
            }
            else
            {
                Graph = graphService.BuildGraph(Steps);
            }
        }
        private bool FilterUsers(UserView user)
        {

            if (user == null)
            {
                return false;
            }
             
            if (ContainsUser(user))
            {
                return false;
            }
            return true;
        }
        private bool ContainsUser(UserView user)
        {
            foreach (var step in Steps)
            {
                if (step.User.Id == user.Id)
                    { return true; }
            }
            return false;
        }
        private void AddStep()
        {
            var newStep = new RouteStep
            {
                Id = SelectedUser.Id.ToString(),
                Name = SelectedUser.Display,
                StepNumber = Steps.Count + 1,
                User = SelectedUser,
                Role = SelectedUser.Role.Name

            };
            Steps.Add(newStep);
            SelectedStep = newStep;
            ApplyFilters();
            Graph = graphService.BuildGraph(Steps);
        }
        private async void LoadUnits()
        {
            Units.Clear();
            var response = await client.From<Unit>().Where(x => x.CompanyId == CurrentUser.CompanyId).Get();
            foreach(var unit in response.Models)
            {
                Units.Add(unit);
            } 
        }

        private async void LoadUsers()
        {
            Users.Clear();
            var response = await client.From<UserView>()
                .Where(x => x.UnitId == SelectedUnit.Id)
                .Where(x => x.RoleId != 2)
                .Where(x => x.RoleId != 1)
                .Get();
            foreach (var user in response.Models) 
            {
                Users.Add(user);
            }

        }
        private void RemoveStep()
        {
            if (SelectedStep != null) 
            {
                Steps.Remove(SelectedStep);
                SelectedStep = null;
                ApplyFilters();
                Graph = graphService.BuildGraph(Steps);
            }
        }
        private void MoveUp()
        {
            int index = Steps.IndexOf(SelectedStep);
            if(index > 0)
            {
                Steps.Move(index, index - 1);
                Graph = graphService.BuildGraph(Steps);
            }
        }
        private void MoveDown() 
        {
            int index = Steps.IndexOf(SelectedStep); 
            if (index < Steps.Count -1 ) 
            {
                Steps.Move(index, index + 1);
                Graph = graphService.BuildGraph(Steps);
            }
        }

        private void SaveRoute()
        {
            IsEnabled = false;
            graphService.SaveRoute(editingRoute, RouteName, Steps);
            UpdateAction();
            MessageBox.Show("Маршрутная карта успешно сохранена!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            IsEnabled = true;
        }

        private async void DeleteRoute()
        {
            var result = MessageBox.Show("Вы точно хотите удалить маршрут?", "Внимание!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                await client.From<ApprovalRoute>().Where(x => x.Id == editingRoute.Id).Delete();
                Steps.Clear();
                RouteName = "";
                UnselectAction();
                UpdateAction();
                Graph = graphService.BuildGraph(Steps);
                OnPropertyChanged();
                editingRoute = null;
            }
        }
    }
}
