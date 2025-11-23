using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagemnetService;
using NLog;
using QuickGraph;
using Supabase;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Media;

namespace DocumentManagementService
{
    
    public class GraphService
    {
        private readonly Client client;
        public ObservableCollection<ApprovalRoute> Routes { get; } = [];
        public ObservableCollection<UserView> Users { get; } = [];
        public GraphService()
        {
            client = App.SupabaseService.Client;
            LoadUsers();
        }
        private async void LoadUsers()
        {
            Users.Clear();
            var response = await client.From<UserView>()
                .Where(x => x.RoleId != 2)
                .Where(x => x.RoleId != 1)
                .Get();
            foreach (var user in response.Models)
            {
                Users.Add(user);
            }

        }
        public BidirectionalGraph<RouteNode, RouteEdge> BuildGraph(ObservableCollection<RouteStep> Steps, ViewDocument document)
        {

            var graph = new BidirectionalGraph<RouteNode, RouteEdge>();

            var nodes = Steps.Select((s, index) => new RouteNode //Преобразование списка шагов в список узлов графа
            {
                Name = s.Name,
                StepNumber = index + 1,
                Id = s.Id,
                Role = s.Role,
                User = s.User,



            }).ToList();

            int currentStep = document.CurrentStepIndex;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (currentStep > 0 && document.Status != "Отклонён")
                {
                    nodes[i].NodeColour = Brushes.LightGreen;
                }
                if (currentStep > 0 && document.Status == "Отклонён")
                {
                    nodes[i].NodeColour = Brushes.Red;
                }
                currentStep--;
                graph.AddVertex(nodes[i]);
            }

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                graph.AddEdge(new RouteEdge(nodes[i], nodes[i + 1])); //Рёбра графа
            }
            ReindexSteps(Steps);
            return graph;
        }
        public BidirectionalGraph<RouteNode, RouteEdge> BuildGraph(ObservableCollection<RouteStep> Steps)
        {

            var graph = new BidirectionalGraph<RouteNode, RouteEdge>();

            var nodes = Steps.Select((s, index) => new RouteNode //Преобразование списка шагов в список узлов графа
            {
                Name = s.Name,
                StepNumber = index + 1,
                Id = s.Id,
                Role = s.Role
               


            }).ToList();

            foreach (var node in nodes)
            {
                graph.AddVertex(node); //Вершины графа
            }
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                graph.AddEdge(new RouteEdge(nodes[i], nodes[i + 1])); //Рёбра графа
            }
            ReindexSteps(Steps);
            return graph;
        }
        public async void SaveRoute(ApprovalRoute editingRoute, string RouteName, ObservableCollection<RouteStep> Steps)
        {
            var nodes = Steps.Select((s, i) => new SerializableRouteNode  //Преобразование списка шагов в список узлов графа для сохранения в таблице
            {
                Id = $"n{i}",
                StepNumber = s.StepNumber,
                Name = s.Name,
                UserId = s.User.Id.ToString(),
                Role = s.Role

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

        }
        private void ReindexSteps(ObservableCollection<RouteStep> Steps) //Автоматическое изменение номера шага (этапа)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].StepNumber = i + 1;
            }
        }
        public async void UpdateRoute(ApprovalRoute editingRoute, ObservableCollection<RouteStep> Steps)
        {
            var model = await client.From<UserView>().Get();
            var Users = model.Models;
            for (int i = 0; i < Steps.Count; i++)
            {
                if (Steps[i].User.Id == Users[i].Id)
                {
                     Steps[i].Name = Users[i].Display;
                     Steps[i].Role = Users[i].Role.Name;
                }
            }
            SaveRoute(editingRoute, editingRoute.Name, Steps);
        }
        public BidirectionalGraph<RouteNode, RouteEdge> LoadRoute(string json, ObservableCollection<RouteStep> Steps)
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
                var user = Users.Where(x => x.Id.ToString() == node.UserId).First();
                var step = new RouteStep 
                { 
                    Name = node.Name,
                    StepNumber = node.StepNumber, 
                    Role = node.Role,
                    User = user,
                    
                };
                Steps.Add(step);
                idToStep[node.Id] = step;
            }

            return BuildGraph(Steps);
        }
        public BidirectionalGraph<RouteNode, RouteEdge> LoadRoute(string json, ObservableCollection<RouteStep> Steps, ViewDocument document)
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
                var user = Users.Where(x => x.Id.ToString() == node.UserId).First();
                var step = new RouteStep
                {
                    Name = node.Name,
                    StepNumber = node.StepNumber,
                    Role = node.Role,
                    User = user,

                };
                Steps.Add(step);
                idToStep[node.Id] = step;
            }

            return BuildGraph(Steps, document);
        }
        public async void LoadRoutes()
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
    }
}
