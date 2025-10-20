using DocumentManagementService.Models;
using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using PdfiumViewer;
using Supabase;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;


namespace DocumentManagementService.Views
{
    public partial class ViewerView : UserControl
    {
        private readonly Client client;

        private PdfViewer pdfViewer;

        private Stream pdfStream;

        private ViewDocument SelectedDocument;
        public ViewerView()
        {
            InitializeComponent();
            ViewerViewModel vm = new();
            DataContext = vm;
            client = App.SupabaseService.Client;
            SelectedDocument = App.SelectedDocument;
            ConvertWordDocToXPSDoc();
           
        }
        public async void ConvertWordDocToXPSDoc()
        {

            var url = await client.Storage.From("documents").CreateSignedUrl(SelectedDocument.Url, 60);

            using var httpClient = new HttpClient();
            byte[] pdfBytes = await httpClient.GetByteArrayAsync(url);

            // Сохраняем ссылку на поток для последующего освобождения
            pdfStream = new MemoryStream(pdfBytes);
            var document = PdfDocument.Load(pdfStream);

            pdfViewer = new PdfViewer
            {
                Document = document,
                Dock = DockStyle.Fill,
            };

            pdfHost.Child = pdfViewer;

        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            pdfHost.Child = null;
            pdfViewer?.Document?.Dispose();
            pdfViewer?.Dispose();
            pdfStream?.Dispose();

            pdfViewer = null;
            pdfStream = null;
        }
    }
}
