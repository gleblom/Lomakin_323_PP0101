using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
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
        private readonly GraphService graphService;

        public Action UnsetAction { get; set; }

        public ICommand CreateRouteCommand { get; }
        public ICommand OnApproveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditRouteCommand { get; }
       
        

        private string searchQuery;
        public string SearchQuery
        {
            get { return searchQuery; }
            set 
            { 
                searchQuery = value; 
                OnPropertyChanged(); 
            }
        }

        public ObservableCollection<Category> Categories { get; } = [];
        public ObservableCollection<ApprovalRoute> Routes { get; } = [];
        public ObservableCollection<RouteStep> Steps { get; } = [];

        private IBidirectionalGraph<RouteNode, RouteEdge> graph;
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph
        {
            get { return graph; }
            set 
            { 
                graph = value;
                OnPropertyChanged();
            }
        }
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
                       Graph = graphService.LoadRoute(selectedRoute?.GraphJson, Steps);
                       //graphService.UpdateRoute(selectedRoute);
                    }
                }             
            } 
        }


        public RoutesViewModel() {
            CreateRouteCommand = new RelayCommand(ConfirmSelection, obj => !App.IsWindowOpen<RouteEditorWindow>());
            EditRouteCommand = new RelayCommand(OpenRoutesEditorWindow, obj => selectedRoute != null && !App.IsWindowOpen<RouteEditorWindow>());
            OnApproveCommand = new RelayCommand(OnApprove, obj => selectedRoute != null);
            CancelCommand = new RelayCommand(Back);

            client = App.SupabaseService.Client;
            documentService = new();
            document = App.SelectedDocument;
            graphService = new();
            navigationService = App.NavigationService;

            LoadRoutes();
            LoadUserInfo();
            LoadCategories();

 
        }
        public async void LoadCategories()
        {
            Categories.Clear();
            var categories = await client.From<Category>().Get();
            foreach (var category in categories.Models)
            {
                category.IsChecked = true;
                Categories.Add(category);
            }
        }
        public void BuildGraph()
        {
            Graph = graphService.BuildGraph(Steps);
            OnPropertyChanged(nameof(Graph));
        }
        private void Unselect()
        {
            SelectedRoute = null;
            Steps.Clear();
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
            if (user.RoleId == 2)
            {
                var window = new RouteEditorWindow();
                RouteEditorViewModel vm = new(graphService, selectedRoute);
                window.DataContext = vm;
                vm.UpdateAction ??= new Action(LoadRoutes);
                vm.UnselectAction ??= new Action(Unselect);
                vm.RouteName = selectedRoute.Name;
                window.Show();
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
            RouteEditorViewModel vm = new(graphService);    
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
           document.Status = "На согласовании";
           await documentService.OnApprove(document, SelectedRoute);
           UnsetAction();
            MessageBox.Show("Документ отправлен на согласование", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
