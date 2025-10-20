using DocumentManagementService.Models;
using DocumentManagemnetService;
using Microsoft.Office.Interop.Word;
using PdfiumViewer;
using Supabase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Xps.Packaging;
using Document = DocumentManagementService.Models.Document;

namespace DocumentManagementService.ViewModels
{
    class ViewerViewModel: BaseViewModel
    {
        private readonly Client client;
        public ObservableCollection<MenuItemModel> DocumentItems { get; }
        public ViewDocument SelectedDocument { get; set; }
        private PdfDocument document;
        public PdfDocument Document
        {
            get { return document; }
            set
            {
                if(document != null)
                {
                    document = value;
                    OnPropertyChanged(nameof(document)); 
                }

            }
        }
        public ViewerViewModel()
        {
            client = App.SupabaseService.Client;
            SelectedDocument = App.SelectedDocument;
 
            DocumentItems =
                [
                    new("Просмотр", "Image", "View"),
                    new("Связанные документы",  "LinkVariant", "Link"),
                    new("Комментарии", "CommentOutline", "Comments"),
                    new("Прошлые версии", "ClockOutline", "Versions"),
                    new("Согласование", "Check", "Approvement")
                ];
        }
    }
}
