using DocumentManagementService.Models;
using DocumentManagemnetService;
using Microsoft.Win32;
using Supabase;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using Document = DocumentManagementService.Models.Document;


namespace DocumentManagementService.ViewModels
{
    public class ApprovalViewModel: BaseViewModel
    {
        private readonly Client client;
        private readonly DocumentService documentService;

        public ObservableCollection<Document> Documents { get; } = []; //Наблюдаемая коллекция для привязки данных
        public Document SelectedDocument { get; set; }
        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand DownloadCommand {  get; }
        public ApprovalViewModel()
        {
            client = App.SupabaseService.Client;
            RejectCommand = new RelayCommand(RejectAsync, obj => SelectedDocument != null);
            ApproveCommand = new RelayCommand(ApproveAsync, obj => SelectedDocument != null);
            DownloadCommand = new RelayCommand(Download, obj => SelectedDocument != null);
            documentService = new DocumentService(client);
            LoadDocuments();
        }
        private async void LoadDocuments() //Загрузка документов для согласования
        {
            List<Notification> notifications = await documentService.GetNotificationsAsync(); 
            Documents.Clear();
            foreach (var notification in notifications)
            {
                var document = await client.From<Document>().
                    Where(x => x.Id == notification.DocumentId). 
                    Get();
                Documents.Add(document.Model);
            }
        }
        private async void Download() //Скачивание документа
        {
            var url = await client.Storage.From("documents").CreateSignedUrl(SelectedDocument.Url, 60); //Получение специальной url для скачивания

            var dialog = new SaveFileDialog
            {
                FileName = Path.GetFileName(SelectedDocument.Url), //Выбор места сохранения
                Filter = "Все файлы|*.*" //Все файлы т.к документа может быть pdf, docx и т.д.
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetStreamAsync(url);

                using (var fileStream = File.Create(dialog.FileName))  
                {
                    await response.CopyToAsync(fileStream); //Сохранение файла
                }
            }
            MessageBox.Show("Файл сохранен!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void DeleteRow()
        {
            Documents.Remove(SelectedDocument);
        }
        private async void ApproveAsync()
        {
            await documentService.ApproveCurrentStepAsync(SelectedDocument, true);
            DeleteRow();
        }
        private async void RejectAsync()
        {
            await documentService.ApproveCurrentStepAsync(SelectedDocument, false);
            DeleteRow();
        }
    }
}
