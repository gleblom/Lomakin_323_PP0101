using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagemnetService;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RouteEditorViewModel: BaseViewModel
    {
        private readonly Client client;
        private ApprovalRoute? editingRoute;
        public ObservableCollection<RouteStep> Steps { get; } = []; //Список этапов (шагов) для построения графа
        public ObservableCollection<User> Users { get; } = [];
        public RouteStep SelectedStep { get; set;  }
        public Action UpdateAction { get; set; } //Делегат для обновления списка маршрутов на RoutesView
        public Action UnselectAction { get; set; } //Делегат для обнуления выбранного
        public string RouteName {  get; set; }
        public User SelectedUser { get; set; }
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph { get; set; } //Граф для отображения шагов маршута
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        


        public RouteEditorViewModel(ApprovalRoute? route = null)
        {
            client = App.SupabaseService.Client;
            editingRoute = route;

            AddStepCommand = new RelayCommand(AddStep, obj => SelectedUser != null);
            RemoveStepCommand = new RelayCommand(RemoveStep, obj => SelectedStep != null);
            MoveUpCommand = new RelayCommand(MoveUp, obj => SelectedStep != null);
            MoveDownCommand = new RelayCommand(MoveDown, obj => SelectedStep != null);
            SaveCommand = new RelayCommand(SaveRoute, obj => (RouteName != null && Steps.Count > 1));
            DeleteCommand = new RelayCommand(DeleteRoute, obj => editingRoute != null);

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
            var newStep = new RouteStep 
            {
                Id = SelectedUser.Id.ToString(), 
                Name = SelectedUser.Display, 
                StepNumber = Steps.Count + 1,
                UserId = SelectedUser.Id
            };
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

        var nodes = Steps.Select((s, index) => new RouteNode //Преобразование списка шагов в список узлов графа
        {
            Name = s.Name,          
            StepNumber = index + 1,
            Id = s.Id,
            UserId = s.UserId
                     
         }).ToList(); 

            foreach (var node in nodes) 
            {
                graph.AddVertex(node); //Вершины графа
            }
            for (int i = 0; i < nodes.Count - 1; i++) 
            {
                graph.AddEdge(new RouteEdge(nodes[i], nodes[i + 1])); //Рёбра графа
            }
            Graph = graph;
            OnPropertyChanged(nameof(Graph));
            ReindexSteps();
        }
        private void ReindexSteps() //Автоматическое изменение номера шага (этапа)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].StepNumber = i + 1;
            }
        }

        private async void SaveRoute()
        {

            var nodes = Steps.Select((s, i) => new SerializableRouteNode  //Преобразование списка шагов в список узлов графа для сохранения в таблице
            {
                Id = $"n{i}",
                StepNumber = s.StepNumber,
                Name = s.Name,
                UserId = s.Id

            }).ToList();

            var edges = new List<SerializableRouteEdge>(); 
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                edges.Add(new SerializableRouteEdge //Сохранение рёбер графа
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

            var json = JsonSerializer.Serialize(graphDto); //Сериализация графа в JSON для хранения в таблице
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
                await client.From<ApprovalRoute>().Update(route); //Обновление маршрута, если редактировали существующий
            }
            else
            {
                var response = await client.From<ApprovalRoute>().Insert(route); //Сохранение нового маршрута
            }
            UpdateAction();
            MessageBox.Show("Маршрутная карта успешно сохранена!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void LoadRoute(string json)
        {
            var dto = JsonSerializer.Deserialize<RouteGraph>(json); //Десериализация графа
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
                BuildGraph();
                OnPropertyChanged();
                editingRoute = null;
            }
        }
    }
}
