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
        private readonly DocumentService documentService;
        private readonly INavigationService navigationService;
        private readonly ViewDocument document;

        public ICommand CreateRouteCommand { get; }
        public ICommand OnApproveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditRouteCommand { get; }

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


        public RoutesViewModel(INavigationService navigationService) {
            CreateRouteCommand = new RelayCommand(ConfirmSelection, obj => !App.IsWindowOpen<RouteEditorWindow>());
            EditRouteCommand = new RelayCommand(OpenRoutesEditorWindow, obj => selectedRoute != null && !App.IsWindowOpen<RouteEditorWindow>());
            OnApproveCommand = new RelayCommand(OnApprove, obj => selectedRoute != null);
            CancelCommand = new RelayCommand(Back);

            client = App.SupabaseService.Client;
            documentService = new(client);
            document = App.SelectedDocument;
            this.navigationService = navigationService;

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
        public async Task<User?> LoadUserInfo()
        {
            var model = await client.From<User>().
                 Where(x => x.Email == client.Auth.CurrentUser.Email).
                 Get();
            return model.Model;

        }
        private async void OpenRoutesEditorWindow()
        {
            var user = await LoadUserInfo();
            MessageBox.Show(user.FirstName);
            if (user.Role == 1)
            {
                var window = new RouteEditorWindow();
                RouteEditorViewModel vm = new(selectedRoute);
                window.DataContext = vm;
                vm.UpdateAction ??= new Action(LoadRoutes);
                vm.UnselectAction ??= new Action(Unselect);
                vm.RouteName = selectedRoute.Name;
                window.Show();
            }
        }
        //private async void LoadUserInfo()
        //{
        //    var user = await client.From<User>().
        //          Where(x => x.Email == client.Auth.CurrentUser.Email).
        //          Get();
        //    if (user.Model != null) 
        //    {
        //        if(user.Model.Role == 1)
        //        {
        //            ShowAction(); //Если пользователь админ, отображаем кнопки создания и редактирования
        //        }
        //    }
        //}

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
        private void Back()
        {
            navigationService.Navigate("MyDocuments");
        }
        private async void OnApprove()
        {
           await documentService.OnApprove(document, SelectedRoute);
        }
    }
}
