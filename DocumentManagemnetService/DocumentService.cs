using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagemnetService;
using NLog;
using Supabase;
using System.IO;
using System.Text.Json;


namespace DocumentManagementService
{
    public class DocumentService
    {
        private readonly Client client;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DocumentService(Client client)
        {
            this.client = client;
        }
        public async Task<bool> AddDocumentAsync(string title, int category, int status, string localFilePath)
        {
            var user = client.Auth.CurrentUser;
            if (user == null)
            {
                Logger.Warn("Пользователь null");
                return false;
            }
            var fileName = Path.GetFileName(localFilePath);
            var storagePath = $"{user.Id}/{fileName}/{Guid.NewGuid()}_{fileName}";

            var url = await UploadFileAsync(localFilePath, storagePath);
            if (url == null)
            {
                Logger.Warn("Url не существует");
                return false;
            }

            var document = new Document
            {
                Title = title,
                Category = category,
                CreatedAt = DateTime.Now,
                AuthorId = user.Id,
                Status = status,
                Url = url

            };
            var response = await client.From<Document>().Insert(document);

            Logger.Info($"{response.Content}");
            return response.Models.Count == 1;
        }
        public async Task<bool> Update(string localFilePath, ViewDocument document)
        {
            var user = client.Auth.CurrentUser;

            var fileName = Path.GetFileName(localFilePath);

            var storagePath = $"{user.Id}/{fileName}/{Guid.NewGuid()}_{fileName}";

            var url = UploadFileAsync(localFilePath, storagePath);

            if (url == null)
            {
                return false;
            }
            var update = await client.From<Document>()
                .Where(x => x.Id == document.Id)
                .Set(x => x.Url, url)
                .Update();

            Logger.Info($"{update.Content}");
            return update.Models.Count == 1;
        }
        public async Task OnApprove(ViewDocument document, ApprovalRoute route)
        {
            if (document.Status == "На согласовании")
            {
                var dto = JsonSerializer.Deserialize<RouteGraph>(route.GraphJson); //Из маршрута получаем граф


                await client.Rpc("InsertNotification", //После отправки на солгласованме уведомляем первого подписанта
                          new Dictionary<string, object> {
                        {"UserId",  dto.Nodes[document.CurrentStepIndex].UserId}, //Из десериализованного графа получаем id первого подписанта
                        {"DocumentId",   document.Id}
                });
                var update = await client.From<Document>()
                    .Where(x => x.Id == document.Id)
                    .Set(x => x.Status, 2)
                    .Set(x => x.RouteId, route.Id)
                    .Set(x => x.CurrentStepIndex, 0)
                    .Update();
                Logger.Info($"{update.Content}");
            }
        }
        public async Task<string?> UploadFileAsync(string localFilePath, string storageFilePath)
        {
            try
            {
                await client.Storage.From("documents").Upload(localFilePath, storageFilePath, new Supabase.Storage.FileOptions
                {
                    Upsert = true
                });
                Logger.Info($"Url файла {storageFilePath}");
                return storageFilePath;
            }
            catch(Exception ex) 
            {
                Logger.Error(ex);
                return null;
            }
        }
        public async Task<List<Notification>> GetNotificationsAsync()
        {
            try
            {
                var notification = await client.From<Notification>().Get();
                return notification.Models;
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return null;
            }

        }
        public async Task<ApprovalRoute> GetApprovalRouteById(Guid? routeId)
        {
            try
            {
                var route = await client.From<ApprovalRoute>().Where(x => x.Id == routeId).Get();
                return route.Model;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }
        public async Task<bool> ApproveCurrentStepAsync(ViewDocument doc, bool approved, string? comment)
        {
            var route = await GetApprovalRouteById(doc.RouteId);
            if (route == null)
            {
                Logger.Warn("Маршрут не прикреплён");
                return false;
            }

            var graph = JsonSerializer.Deserialize<RouteGraph>(route.GraphJson);
            var steps = graph.Nodes.OrderBy(n => n.Id).ToList();

            var currentIndex = doc.CurrentStepIndex;
            var currentNode = steps.ElementAtOrDefault(currentIndex);
            if (currentNode == null)
            {
                return false;
            }

            var currentUserId = client.Auth.CurrentUser?.Id;


            var approval = new DocumentApprovals
            {
                Id = Guid.NewGuid(),
                DocumentId = doc.Id,
                UserId = currentUserId,
                StepIndex = currentIndex,
                IsApproved = approved,
                ApprovedAt = DateTime.UtcNow,
                Comment = comment
            };
            await client.From<DocumentApprovals>().Insert(approval);

            var model = await client.From<Document>().Where(x => x.Id == doc.Id).Single();

            if (!approved)
            {
                model.Status = 1;
                
                await client.From<Document>().Update(model);

                Logger.Info("Документ отклонен");
            }
            else if (currentIndex >= steps.Count - 1)
            {

                model.Status = 3;
                model.CurrentStepIndex = currentIndex + 1;

                await client.From<Document>().Update(model);

                Logger.Info("Документ согласован");
            }
            else
            {
                model.Status = 2;
                model.CurrentStepIndex = currentIndex + 1;

                var nextNode = steps[model.CurrentStepIndex];

                await client.From<Document>().Update(model);

                await client.Rpc("InsertNotification",
                    new Dictionary<string, object> {
                        {"UserId", nextNode.UserId},
                        {"DocumentId",  doc.Id}
                    });
                Logger.Info($"Этап согласован, текущий индекс: {model.CurrentStepIndex}");
            }
            await client.From<Notification>().
            Where(x => x.DocumentId == doc.Id).
            Delete();


            return true;
        }
    }
}
