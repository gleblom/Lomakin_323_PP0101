using DocumentManagementService.Models;
using DocumentManagementService.Views;
using DocumentManagemnetService;
using Microsoft.Win32;
using QuickGraph;
using Supabase;
using Supabase.Gotrue;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Document = DocumentManagementService.Models.Document;


namespace DocumentManagementService.ViewModels
{
    public class ApprovalViewModel: BaseViewModel
    {

        private readonly Supabase.Client client;
        private readonly DocumentService documentService;
        private readonly GraphService graphService;
        public ViewDocument SelectedDocument { get; set; }
        
        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public ObservableCollection<RouteStep> Steps { get; } = [];


        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {

                visibility = value;
                OnPropertyChanged();
            }
        }

        private IBidirectionalGraph<RouteNode, RouteEdge> graph;
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                OnPropertyChanged();
            }
        }
        public ApprovalViewModel()
        {

            client = App.SupabaseService.Client;
            SelectedDocument = App.SelectedDocument;
            documentService = new DocumentService(client);
            graphService = new GraphService(App.Users);

   
            if(SelectedDocument.Status == "Опубликован" || SelectedDocument.Status == "Не согласован")
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
            RejectCommand = new RelayCommand(RejectAsync, 
                obj => SelectedDocument != null && !App.IsWindowOpen<CommentWindow>());
            ApproveCommand = new RelayCommand(ApproveAsync, 
                obj => SelectedDocument != null && !App.IsWindowOpen<CommentWindow>());


            BuildGraph(SelectedDocument);
        }
        private async Task<ApprovalRoute?> LoadRoute()
        {
            var route = await client.From<ApprovalRoute>()
                 .Where(x => x.Id == SelectedDocument.RouteId).Get();


            return route.Model;
        }
        private async void BuildGraph(ViewDocument document)
        {
            var route = await LoadRoute();
            Graph = graphService.LoadRoute(route.GraphJson, Steps, document);
        }

        private async void ApproveCurrentStep(bool isApprove, string? comment)
        {
            await documentService.ApproveCurrentStepAsync(SelectedDocument, isApprove, comment);

            var doc = await client.From<ViewDocument>()
                 .Where(x => x.Id == SelectedDocument.Id).Get();

            MessageBox.Show($"{doc.Model.Status}");
            MessageBox.Show($"{doc.Model.CurrentStepIndex}");

            BuildGraph(doc.Model);

            visibility = Visibility.Collapsed;
        }
        private void ApproveAsync()
        {
            ApproveCurrentStep(true, null);
        }
        private void RejectAsync()
        {
            CommentWindow commentWindow = new();
            CommentViewModel vm = new();
            vm.AddCommentAction ??= AddComment;
            vm.CancelAction ??= new Action(commentWindow.Close);
            commentWindow.DataContext = vm;
            commentWindow.Show();
        }
        public void AddComment(string comment) => ApproveCurrentStep(false, comment);

    }
}
