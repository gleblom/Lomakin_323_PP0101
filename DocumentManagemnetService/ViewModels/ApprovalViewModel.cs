using DocumentManagementService.Models;
using DocumentManagemnetService;
using Microsoft.Win32;
using QuickGraph;
using Supabase;
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
        private readonly Client client;
        private readonly DocumentService documentService;
        private readonly GraphService graphService;
        public ViewDocument SelectedDocument { get; set; }
        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddCommentCommand { get; }
        public ObservableCollection<RouteStep> Steps { get; } = [];

        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                if(SelectedDocument.Status == "Опубликован")
                {
                    visibility = Visibility.Collapsed;
                }
                else
                {
                    visibility = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }
        private bool isOpen = false;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                OnPropertyChanged();
            }
        }

        private string comment;
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
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
            documentService = new DocumentService();
            graphService = new GraphService();

   
            RejectCommand = new RelayCommand(RejectAsync, obj => SelectedDocument != null);
            ApproveCommand = new RelayCommand(ApproveAsync, obj => SelectedDocument != null);
            CancelCommand = new RelayCommand(Cancel);
            AddCommentCommand = new RelayCommand(AddComment);

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

            BuildGraph(doc.Model);

            Visibility = Visibility.Collapsed;
        }
        private void ApproveAsync()
        {
            ApproveCurrentStep(true, null);
        }
        private void RejectAsync()
        {
            IsOpen = true;
        }
        private void AddComment()
        {
            ApproveCurrentStep(false, comment);
        }
        private void Cancel()
        {
            IsOpen = false;
        }
    }
}
