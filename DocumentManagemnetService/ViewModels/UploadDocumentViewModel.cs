using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class UploadDocumentViewModel: BaseViewModel
    {
        public string DocumentTitle { get; set; }
        public string SelectedFileName { get; set; }
        public string SelectedFileCategory { get; set; }
        public bool IsDraggingFile { get; set; }

        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>() { "Регламент", "Инструкция", "Методика", "Политика" };
        public ObservableCollection<string> DocumentTypes { get; } = new ObservableCollection<string>() { "PDF", "DOCX", "XLSX" };

        public ICommand SelectFileCommand { get; }
        public ICommand SaveDraftCommand { get; }
        public ICommand SubmitCommand { get; }

        private string selectedFilePath;

        public UploadDocumentViewModel() 
        {
            SelectFileCommand = new RelayCommand(OpenFileDialog);
            SaveDraftCommand = new RelayCommand(SaveAsDraft);
            SubmitCommand = new RelayCommand(SubmitDocument);
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
                OnPropertyChaneged(nameof(SelectedFileName));
            }
        }
        private void SaveAsDraft()
        {

        }
        private void SubmitDocument()
        {

        }
        public void HandleDropFile(string filePath)
        {
            selectedFilePath = filePath;
            SelectedFileName = Path.GetFileName(filePath);
            OnPropertyChaneged(nameof(SelectedFileName));
        }

    }
}
