using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagemnetService;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;

namespace DocumentManagementService
{
    
    public class GraphService
    {
        private readonly Client client;
        public ObservableCollection<RouteStep> Steps { get; } = [];
        public GraphService()
        {
            client = App.SupabaseService.Client;
        }
        private BidirectionalGraph<RouteNode, RouteEdge> BuildGraph()
        {

            var graph = new BidirectionalGraph<RouteNode, RouteEdge>();

            var nodes = Steps.Select((s, index) => new RouteNode //Преобразование списка шагов в список узлов графа
            {
                Name = s.Name,
                StepNumber = index + 1,
                Id = s.Id,


            }).ToList();

            foreach (var node in nodes)
            {
                graph.AddVertex(node); //Вершины графа
            }
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                graph.AddEdge(new RouteEdge(nodes[i], nodes[i + 1])); //Рёбра графа
            }
            return graph;
            //OnPropertyChanged(nameof(Graph));
            //ReindexSteps();
        }
        private async void SaveRoute(ApprovalRoute editingRoute, string RouteName)
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
            if (userId == null)
            {
                return;
            }

            var route = editingRoute ?? new ApprovalRoute
            {
                Id = Guid.NewGuid(),
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
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
            //UpdateAction();
            //MessageBox.Show("Маршрутная карта успешно сохранена!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public async void UpdateRoute(ApprovalRoute editingRoute)
        {
            var model = await client.From<User>().Get();
            var Users = model.Models;
            for (int i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].UserId == Users[i].Id)
                {
                    if (Steps[i].Name != Users[i].Display)
                    {
                        Steps[i].Name = Users[i].Display;
                    }
                }
            }
            SaveRoute(editingRoute, editingRoute.Name);
        }
        private BidirectionalGraph<RouteNode, RouteEdge> LoadRoute(string json)
        {
            var dto = JsonSerializer.Deserialize<RouteGraph>(json); //Десериализация графа
            if (dto is null)
            {
                return null;
            }

            Steps.Clear();
            var idToStep = new Dictionary<string, RouteStep>();

            foreach (var node in dto.Nodes)
            {
                var step = new RouteStep { Name = node.Name, StepNumber = node.StepNumber };
                Steps.Add(step);
                idToStep[node.Id] = step;
            }

            return BuildGraph();
        }
        private async void DeleteRoute(ApprovalRoute editingRoute)
        {
            var result = MessageBox.Show("Вы точно хотите удалить маршрут?", "Внимание!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                await client.From<ApprovalRoute>().Where(x => x.Id == editingRoute.Id).Delete();
                //Steps.Clear();
                //RouteName = "";
                //UnselectAction();
                //UpdateAction();
                //BuildGraph();
                //OnPropertyChanged();
                //editingRoute = null;
            }
        }
    }
}
