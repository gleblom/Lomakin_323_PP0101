using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using Microsoft.Win32;
using Supabase;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class UploadDocumentViewModel: BaseViewModel
    {
        private readonly Client client;
        private readonly DocumentService documentService;
        private readonly User currentUser;
        public string SelectedFileName { get; set; }
        public bool IsDraggingFile { get; set; }
        public ObservableCollection<Category> Categories { get; } = [];
        public ICommand SelectFileCommand { get; }
        public ICommand SubmitCommand { get; }
        private string selectedFilePath;
        public const string BasicTextPattern = @"^[a-zA-Zа-яА-Я0-9\s\.,!?;:""'\(\)\-]*$";

        private string documentTitle;
        public string DocumentTitle
        {
            get { return documentTitle; }
            set
            {
                if(documentTitle != value)
                {
                    documentTitle = value;
                    OnPropertyChanged(nameof(documentTitle));
                }
            }
        }

        private Category category;
        public Category SelectedFileCategory
        {
            get {  return category; }
            set
            { 
                if(category != value)
                {
                    category = value;
                    OnPropertyChanged(nameof(category));
                }
            }
        }
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    OnPropertyChanged(nameof(isEnabled));
                }
            }
        }

        public UploadDocumentViewModel(DocumentService documentService) 
        {
            SelectFileCommand = new RelayCommand(OpenFileDialog);
            currentUser = App.CurrentUser;

            var regex = new Regex(BasicTextPattern, RegexOptions.Compiled);

            SubmitCommand = new RelayCommand(SubmitDocument, obj => 
            (SelectedFileCategory != null && IsEnabled
            && DocumentTitle != null && regex.IsMatch(DocumentTitle)
            && SelectedFileName != null 
            && !App.IsWindowOpen<SelectRouteView>()));

            client = App.SupabaseService.Client;
            this.documentService = documentService;

            LoadCategries();
        }
        public async void LoadCategries()
        {
            Categories.Clear();
            var categories = await client.From<RoleCategory>()
                .Where(x => x.RoleId == currentUser.RoleId).Get();
            foreach (var category in categories.Models) 
            {
                Categories.Add(category.Category);
            }
        }
        private void OpenFileDialog()
        {
            var dialoig = new OpenFileDialog()
            {
                Filter = "Документы (*.pdf;*.docx)|*.pdf;*.docx",
                Multiselect = false
            };
            if (dialoig.ShowDialog() == true) 
            {
                selectedFilePath = dialoig.FileName;
                SelectedFileName = Path.GetFileName(selectedFilePath);
                OnPropertyChanged(nameof(SelectedFileName));
            }
        }
        private async void SubmitDocument()
        {
            IsEnabled = false;
            bool success = await documentService.AddDocumentAsync(DocumentTitle, SelectedFileCategory.Id, 1, selectedFilePath);
            if (success)
            {
                MessageBox.Show("Документ сохранен", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении документа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            IsEnabled = true;
        }
        public void HandleDropFile(string filePath)
        {
            selectedFilePath = filePath;
            SelectedFileName = Path.GetFileName(filePath);
            OnPropertyChanged(nameof(SelectedFileName));
        }

    }
}
