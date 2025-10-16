using DocumentManagementService.Models;
using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using PdfiumViewer;
using Supabase;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using UserControl = System.Windows.Controls.UserControl;


namespace DocumentManagementService.Views
{
    public partial class ViewerView : UserControl
    {
        private readonly Client client;

        private PdfViewer _pdfViewer;
        public Document SelectedDocument { get; set; }
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

            using var stream = new MemoryStream(pdfBytes);
            var document = PdfDocument.Load(stream);

            _pdfViewer = new PdfViewer
            {
                Document = document,
                Dock = DockStyle.Fill,
            };
            var viewerHost = pdfHost;
            viewerHost.Child = _pdfViewer;

        }
    }
}
