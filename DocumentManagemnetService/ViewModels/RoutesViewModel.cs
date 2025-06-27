using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RoutesViewModel : BaseViewModel
    {
        private readonly Client client;
        public ICommand CreateRouteCommand { get; }
        public ObservableCollection<ApprovalRoute> Routes { get; } = [];
        public ObservableCollection<RouteStep> Steps { get; } = [];
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph { get; set; }
        public Action ShowAction { get; set; }
        private ApprovalRoute selectedRoute;
        public ApprovalRoute? SelectedRoute
        {
            get { return selectedRoute;}
            set 
            {
                if (selectedRoute != value) 
                {
                    selectedRoute = value;
                    OnPropertyChanged();
                    if (selectedRoute != null)
                    {
                        LoadRoute(selectedRoute?.GraphJson);
                    }
                }             
            } 
        }
        public ICommand EditRouteCommand { get; }

        public RoutesViewModel() {
            CreateRouteCommand = new RelayCommand(ConfirmSelection, obj => !App.IsWindowOpen<RouteEditorWindow>());
            EditRouteCommand = new RelayCommand(OpenRoutesEditorWindow, obj => selectedRoute != null && !App.IsWindowOpen<RouteEditorWindow>());

            client = App.SupabaseService.Client;


            LoadRoutes();

            LoadUserInfo();
        }
        public void BuildGraph()
        {
            var graph = new BidirectionalGraph<RouteNode, RouteEdge>();

            var nodes = Steps.Select((s, index) => new RouteNode
            {
                Name = s.Name,
                StepNumber = index + 1,
                Id = s.Id,


            }).ToList();

            foreach (var node in nodes)
            {
                graph.AddVertex(node);
            }
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                graph.AddEdge(new RouteEdge(nodes[i], nodes[i + 1]));
            }
            Graph = graph;
            OnPropertyChanged(nameof(Graph));
        }
        private void Unselect()
        {
            SelectedRoute = null;
            Steps.Clear();
            BuildGraph();
        }
        private void LoadRoute(string json)
        {
            var dto = JsonSerializer.Deserialize<RouteGraph>(json);
            if (dto is null)
            {
                return;
            }

            Steps.Clear();
            var idToStep = new Dictionary<string, RouteStep>();

            foreach (var node in dto.Nodes)
            {
                var step = new RouteStep { Name = node.Name, StepNumber = node.StepNumber };
                Steps.Add(step);
                idToStep[node.Id] = step;
            }

            BuildGraph();
        }
        private void OpenRoutesEditorWindow()
        {
            var window = new RouteEditorWindow();
            RouteEditorViewModel vm = new(selectedRoute);
            window.DataContext = vm;
            vm.UpdateAction ??= new Action(LoadRoutes);
            vm.UnselectAction ??= new Action(Unselect);
            vm.RouteName = selectedRoute.Name;
            window.Show();
        }
        private async void LoadUserInfo()
        {
            var user = await client.From<User>().
                  Where(x => x.Email == client.Auth.CurrentUser.Email).
                  Get();
            if (user.Model != null) 
            {
                if(user.Model.Role == "admin")
                {
                    ShowAction(); //Если пользователь админ, отображаем кнопки создания и редактирования
                }
            }
        }

        private async void LoadRoutes()
        {
            var response = await client
                .From<ApprovalRoute>()
                .Select("*")
                .Get();

            Routes.Clear();
            foreach (var route in response.Models)
            {
                Routes.Add(route);
            }
        }
        private void ConfirmSelection() 
        {

            var window = new RouteEditorWindow();
            RouteEditorViewModel vm = new();    
            window.DataContext = vm;
            vm.UpdateAction ??= new Action(LoadRoutes);
            vm.UnselectAction ??= new Action(Unselect);
            window.Show();
        }
    }
}
