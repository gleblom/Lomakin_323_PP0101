using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagemnetService;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RouteEditorViewModel: BaseViewModel
    {
        private readonly Client client;
        private ApprovalRoute? editingRoute;
        public ObservableCollection<RouteStep> Steps { get; } = [];
        public ObservableCollection<User> Users { get; } = [];
        public RouteStep SelectedStep { get; set;  }
        public string RouteName {  get; set; }
        public User SelectedUser { get; set; }
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph { get; set; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        


        public RouteEditorViewModel(Client client, ApprovalRoute? route = null)
        {
            this.client = client;
            this.editingRoute = route;

            AddStepCommand = new RelayCommand(AddStep, obj => SelectedUser != null);
            RemoveStepCommand = new RelayCommand(RemoveStep, obj => SelectedStep != null);
            MoveUpCommand = new RelayCommand(MoveUp, obj => SelectedStep != null);
            MoveDownCommand = new RelayCommand(MoveDown, obj => SelectedStep != null);
            SaveCommand = new RelayCommand(SaveRoute, obj => (RouteName != null && Steps.Count > 1));
            DeleteCommand = new RelayCommand(DeleteRoute);

            LoadUsers();

            if (editingRoute != null)
            {
                LoadRoute(editingRoute.GraphJson);
            }
            else
            {
                BuildGraph();
            }
        }
        private void AddStep()
        {
            var newStep = new RouteStep { Name = SelectedUser.Display, StepNumber = Steps.Count + 1};
            Steps.Add(newStep);
            SelectedStep = newStep;
            BuildGraph();
        }
        private async void LoadUsers()
        {
            var response = await client.From<User>().Select("*").Get();
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
                BuildGraph();
            }
        }
        private void MoveUp()
        {
            int index = Steps.IndexOf(SelectedStep);
            if(index > 0)
            {
                Steps.Move(index, index - 1);
                BuildGraph();
            }
        }
        private void MoveDown() 
        {
            int index = Steps.IndexOf(SelectedStep); 
            if (index < Steps.Count -1 ) 
            {
                Steps.Move(index, index + 1);
                BuildGraph();
            }
        }
        private void BuildGraph()
        {
         var graph = new BidirectionalGraph<RouteNode, RouteEdge>();

        var nodes = Steps.Select((s, index) => new RouteNode
        {
            Name = s.Name,          
            StepNumber = index + 1
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
            ReindexSteps();
        }
        private void ReindexSteps()
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].StepNumber = i + 1;
            }
        }

        private async void SaveRoute()
        {

            var nodes = Steps.Select((s, i) => new SerializableRouteNode
            {
                Id = $"n{i}",
                StepNumber = s.StepNumber,
                Name = s.Name,
                UserId = client.Auth.CurrentUser.Id

            }).ToList();

            var edges = new List<SerializableRouteEdge>();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                edges.Add(new SerializableRouteEdge
                {
                    SourceId = nodes[i].Id,
                    TargetId = nodes[i + 1].Id
                });
            }

            var graphDto = new RouteGraph
            {
                Nodes = nodes,
                Edges = edges
            };

            var json = JsonSerializer.Serialize(graphDto);
            var userId = client.Auth.CurrentUser?.Id;
            if (userId == null) {
                return;
            } 

            var route = editingRoute ?? new ApprovalRoute
            {
                Id = Guid.NewGuid(),
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            route.Name = RouteName;
            route.GraphJson = json;

            if (editingRoute != null)
            {
                await client.From<ApprovalRoute>().Update(route);
            }
            else
            {
                await client.From<ApprovalRoute>().Insert(route);
            }
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
        private void DeleteRoute()
        {

        }
    }
}
