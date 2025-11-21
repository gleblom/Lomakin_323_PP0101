using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using System.Windows;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class UploadDocumentView : UserControl
    {
        public UploadDocumentView()
        {
            InitializeComponent();
            var client = App.SupabaseService.Client;
            DocumentService service = new();
            DataContext = new UploadDocumentViewModel(service);
        }

        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ((UploadDocumentViewModel)DataContext).IsDraggingFile = true;
            }
        }

        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            ((UploadDocumentViewModel)DataContext).IsDraggingFile = false;
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            ((UploadDocumentViewModel)DataContext).IsDraggingFile = false;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    ((UploadDocumentViewModel)DataContext).HandleDropFile(files[0]);
                }
            }
        }
    }
}
