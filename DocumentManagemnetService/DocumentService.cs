using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using DocumentManagemnetService;
using Supabase;
using System.IO;
using System.Text.Json;


namespace DocumentManagementService
{
    public class DocumentService
    {
        private readonly Client client;

        public DocumentService()
        {
            client = App.SupabaseService.Client;
        }
        public async Task<bool> AddDocumentAsync(string title, int category, int status, string localFilePath)
        {
            var user = client.Auth.CurrentUser;
            if (user == null)
            {
                return false;
            }
            var fileName = Path.GetFileName(localFilePath);
            var storagePath = $"{user.Id}/{fileName}/{Guid.NewGuid()}_{fileName}";

            var url = await UploadFileAsync(localFilePath, storagePath);
            if (url == null)
            {
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
            //if(status == 2)
            //{
            //    var dto = JsonSerializer.Deserialize<RouteGraph>(route.GraphJson); //Из маршрута получаем граф


            //    await client.Rpc("InsertNotification", //После отправки на солгласованме уведомляем первого подписанта
            //              new Dictionary<string, object> {
            //            {"UserId",  dto.Nodes[document.CurrentStepIndex].UserId}, //Из десериализованного графа получаем id первого подписанта
            //            {"DocumentId",   response.Model.Id}
            //    });

            //}

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
                return storageFilePath;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<Notification>> GetNotificationsAsync()
        {
            var notification = await client.From<Notification>().Get();
            return notification.Models;
        }
        public async Task<ApprovalRoute> GetApprovalRouteById(Guid? routeId)
        {
            var route = await client.From<ApprovalRoute>().Where(x => x.Id == routeId).Get();
            return route.Model;
        }
        public async Task<bool> ApproveCurrentStepAsync(ViewDocument doc, bool approved, string? comment)
        {
            var route = await GetApprovalRouteById(doc.RouteId);
            if (route == null)
            {
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
                IsApproved = true,
                Comment = comment
            };
            await client.From<DocumentApprovals>().Insert(approval);

            var model = await client.From<Document>().Where(x => x.Id == doc.Id).Single();

            if (!approved)
            {
                model.Status = 1;

                await client.From<Document>().Update(model);
            }
            else if (currentIndex >= steps.Count - 1)
            {

                model.Status = 3;
                model.CurrentStepIndex = currentIndex + 1;

                await client.From<Document>().Update(model);

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
            }
            await client.From<Notification>().
            Where(x => x.DocumentId == doc.Id).
            Delete();


            return true;
        }
    }
}
