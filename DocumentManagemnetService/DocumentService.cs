using DocumentManagementService.Models;
using Supabase;
using System.IO;
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
        public async Task<bool> AddDocumentAsync(string title, string category, string status, string localFilePath)
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
                Url = url
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
    }
}
