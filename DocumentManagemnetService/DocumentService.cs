using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using Supabase;
using System.IO;
using System.Text.Json;
using System.Windows;


namespace DocumentManagementService
{
    public class DocumentService
    {
        private readonly Client client;

        public DocumentService(Client client) 
        {
            this.client = client; 
        }
        public async Task<bool> AddDocumentAsync(string title, string category, string status, string localFilePath, Guid? key)
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
                RouteId = key
            };
            var response = await client.From<Document>().Insert(document);
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
        public async Task<bool> ApproveCurrentStepAsync(Document doc)
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
            if (currentNode == null) return false;

            var currentUserId = client.Auth.CurrentUser?.Id;
            if (currentUserId != currentNode.UserId)
            {
                return false;
            }


            await client.From("document_approvals").Insert(new
            {
                document_id = doc.Id,
                user_id = currentUserId,
                step_index = currentIndex,
                approved = true
            });

            if (currentIndex >= steps.Count - 1)
            {

                var model = await client.From<Document>().Where(x => x.Id == doc.Id).Single();
                model.Status = "опубликован";
                model.CurrentStepIndex = currentIndex + 1;
                await model.Update<Document>();


                //await MakeDocumentPublic(doc.Id);
            }
            else
            {
                // Переход к следующему этапу
               var model = await client.From<Document>().Where(x => x.Id == doc.Id).Single();
                model.Status = "В процессе согласования";
                model.CurrentStepIndex = currentIndex + 1;
                await model.Update<Document>();
            }

            return true;
        }

    }
}
