using DocumentManagementService.Views;
using DocumentManagemnetService;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
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
        public ICommand SubmitCommand { get; }

        private string selectedFilePath;

        private DocumentService documentService;

        public UploadDocumentViewModel(DocumentService documentService) 
        {
            SelectFileCommand = new RelayCommand(OpenFileDialog);
            SubmitCommand = new RelayCommand(SubmitDocument, obj => 
            (SelectedFileCategory != null 
            && DocumentTitle != null 
            && SelectedFileName != null 
            && !App.IsWindowOpen<SelectRouteView>()));

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
        private void SubmitDocument()
        {
            var selectWindow = new SelectRouteView();
            SelectRouteViewModel vm = new SelectRouteViewModel();
            vm.DocumentTitle = DocumentTitle;
            vm.SelectedFilePath = selectedFilePath;
            vm.SelectedFileCategory = SelectedFileCategory;
            vm.documentService = documentService;
            vm.CloseAction ??= new Action(selectWindow.Close);

            selectWindow.DataContext = vm;

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
