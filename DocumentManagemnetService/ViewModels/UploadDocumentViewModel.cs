using DocumentManagementService.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class UploadDocumentViewModel: BaseViewModel
    {
        public string DocumentTitle { get; set; }
        public string SelectedFileName { get; set; }
        public string SelectedFileCategory { get; set; }
        public bool IsDraggingFile { get; set; }
        public ObservableCollection<string> Categories { get; } = ["Регламент", "Инструкция", "Методика", "Политика"];
        public ICommand SelectFileCommand { get; }
        public ICommand SaveDraftCommand { get; }
        public ICommand SubmitCommand { get; }

        private string selectedFilePath;

        private DocumentService documentService;

        public UploadDocumentViewModel(DocumentService documentService) 
        {
            SelectFileCommand = new RelayCommand(OpenFileDialog);
            SaveDraftCommand = new RelayCommand(SaveAsDraft, obj => (SelectedFileCategory != null && DocumentTitle != null && SelectedFileName != null));
            SubmitCommand = new RelayCommand(SubmitDocument, obj => (SelectedFileCategory != null && DocumentTitle != null && SelectedFileName != null));
            this.documentService = documentService;
        }
        private void OpenFileDialog()
        {
            var dialoig = new OpenFileDialog()
            {
                Filter = "Документы (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx",
                Multiselect = false
            };
            if (dialoig.ShowDialog() == true) 
            {
                selectedFilePath = dialoig.FileName;
                SelectedFileName = Path.GetFileName(selectedFilePath);
                OnPropertyChanged(nameof(SelectedFileName));
            }
        }
        private async void SaveAsDraft()
        {
            bool success = await documentService.AddDocumentAsync(DocumentTitle, SelectedFileCategory, "Черновик", selectedFilePath, null);
            if (success)
            {
                MessageBox.Show("Документ сохранен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void SubmitDocument()
        {
            var selectWindow = new SelectRouteView
            {
                DataContext = new SelectRouteViewModel
                {
                    DocumentTitle = DocumentTitle,
                    SelectedFilePath = selectedFilePath,
                    SelectedFileCategory = SelectedFileCategory,
                    documentService = documentService
                    
                }
            };
            selectWindow.Show();
        }
        public void HandleDropFile(string filePath)
        {
            selectedFilePath = filePath;
            SelectedFileName = Path.GetFileName(filePath);
            OnPropertyChanged(nameof(SelectedFileName));
        }

    }
}
