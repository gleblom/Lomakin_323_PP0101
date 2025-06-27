using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using Supabase;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;


namespace DocumentManagementService
{
    public class DocumentService
    {
        private readonly Client client;

        public DocumentService(Client client) 
        {
            this.client = client; 
        }
        public async Task<bool> AddDocumentAsync(string title, string category, string status, string localFilePath, ApprovalRoute route)
        {
            var user = client.Auth.CurrentUser;
            if (user == null)
            {
                return false;
            }
            var fileName = Path.GetFileName(localFilePath);
            var storagePath = $"{user.Id}/{Guid.NewGuid()}_{fileName}";

            var url = await UploadFileAsync(localFilePath, storagePath);
            if (url == null) {
                return false;
            }

            var document = new Document
            {
                Title = title,
                Category = category,
                CreatedAt = DateTime.Now,
                AuthorId = user.Id,
                Status = status,
                Url = url,
                RouteId = route.Id
            };
            var response = await client.From<Document>().Insert(document);
            if(status == "В процессе согласования")
            {
                var dto = JsonSerializer.Deserialize<RouteGraph>(route.GraphJson); //Из маршрута получаем граф

         
                await client.Rpc("InsertNotification", //После отправки на солгласованме уведомляем первого подписанта
                          new Dictionary<string, object> {
                        {"UserId",  dto.Nodes[document.CurrentStepIndex].UserId}, //Из десериализованного графа получаем id первого подписанта
                        {"DocumentId",   response.Model.Id}
                });

            }
          
            return response.Models.Count == 1;
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
        public async Task<bool> ApproveCurrentStepAsync(Document doc, bool approved)
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
                IsApproved = true
            };
            await client.From<DocumentApprovals>().Insert(approval);

            var model = await client.From<Document>().Where(x => x.Id == doc.Id).Single();

            if (!approved)
            {
                model.Status = "Отклонён";

                await client.From<Document>().Update(model);
            }
            else if (currentIndex >= steps.Count - 1)
            {

                model.Status = "Опубликован";
                model.CurrentStepIndex = currentIndex + 1;

                await client.From<Document>().Update(model);

            }
            else
            {
                model.Status = "В процессе согласования";
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
