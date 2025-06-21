using DocumentManagementService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DocumentManagementService.Views
{
    /// <summary>
    /// Логика взаимодействия для UploadDocumentView.xaml
    /// </summary>
    public partial class UploadDocumentView : UserControl
    {
        public UploadDocumentView()
        {
            InitializeComponent();
            DataContext = new UploadDocumentViewModel();
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
