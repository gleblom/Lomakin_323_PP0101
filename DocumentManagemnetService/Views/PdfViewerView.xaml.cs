using DocumentManagementService.Models;
using DocumentManagemnetService;
using Microsoft.Office.Interop.Word;
using PdfiumViewer;
using Supabase;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using PdfDocument = PdfiumViewer.PdfDocument;
using UserControl = System.Windows.Controls.UserControl;
using Word = Microsoft.Office.Interop.Word;

namespace DocumentManagementService.Views
{
    public partial class PdfViewerView : UserControl
    {
        private readonly Client client;

        private PdfViewer pdfViewer;

        private Stream pdfStream;

        private string pdfFile;

        private string docFile;

        private ViewDocument SelectedDocument;
        public PdfViewerView()
        {
            InitializeComponent();
            SelectedDocument = App.SelectedDocument;
            client = App.SupabaseService.Client;
            ShowDocument();
        }
        private string ConvertWordToPdf(string docxPath)
        {
            string fullPath = Path.GetFullPath(docxPath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Файл не найден: {fullPath}");

            string pdfPath = Path.ChangeExtension(fullPath, ".pdf");

            var wordApp = new Word.Application();
            try
            {
                var doc = wordApp.Documents.Open(fullPath, ReadOnly: true, Visible: false);
                doc.ExportAsFixedFormat(pdfPath, WdExportFormat.wdExportFormatPDF);
                doc.Close(false);
            }
            finally
            {
                wordApp.Quit(false);
            }

            return pdfPath;
        }
        private async Task<string> DownloadFileAsync(string url)
        {
            using var http = new HttpClient();
            var data = await http.GetByteArrayAsync(url);

            string extension = Path.GetExtension(SelectedDocument.Url);
            string tempFile = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}{extension}");
            await File.WriteAllBytesAsync(tempFile, data);

            return tempFile;
        }
        public async void ShowDocument()
        {
            try
            {
                var url = await client.Storage.From("documents").CreateSignedUrl(SelectedDocument.Url, 60);

                string tempFile = await DownloadFileAsync(url);
                string pdfPath;

                if (SelectedDocument.Url.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    pdfPath = tempFile;
                    pdfFile = tempFile;
                }
                else if (SelectedDocument.Url.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) ||
                         SelectedDocument.Url.EndsWith(".doc", StringComparison.OrdinalIgnoreCase))
                {
                    docFile = tempFile;
                    pdfPath = ConvertWordToPdf(tempFile);
                    pdfFile = pdfPath;

                }
                else
                {
                    MessageBox.Show("Неподдерживаемый формат файла. Поддерживаются .pdf, .doc, .docx");
                    return;
                }

                var document = PdfDocument.Load(pdfPath);
                pdfViewer = new PdfViewer
                {
                    Document = document,
                    Dock = DockStyle.Fill
                };

                pdfHost.Child = pdfViewer;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading document: {ex.Message}");
            }
        }




        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            pdfHost.Child = null;
            pdfViewer?.Document?.Dispose();
            pdfViewer?.Dispose();
            pdfStream?.Dispose();

            File.Delete(pdfFile);
            File.Delete(docFile);

            pdfViewer = null;
            pdfStream = null;
        }
    }
}
