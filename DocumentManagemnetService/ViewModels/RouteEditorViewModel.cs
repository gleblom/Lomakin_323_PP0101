using DocumentManagementService.Models;
using QuickGraph;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class RouteEditorViewModel: BaseViewModel
    {
        public ObservableCollection<RouteStep> Steps { get; } = [];
        public RouteStep SelectedStep { get; set;  }
        public IBidirectionalGraph<RouteNode, RouteEdge> Graph { get; set; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }


        public RouteEditorViewModel()
        {
            AddStepCommand = new RelayCommand(AddStep);
            RemoveStepCommand = new RelayCommand(RemoveStep, obj => SelectedStep != null);
            MoveUpCommand = new RelayCommand(MoveUp, obj => SelectedStep != null);
            MoveDownCommand = new RelayCommand(MoveDown, obj => SelectedStep != null);
            SaveCommand = new RelayCommand(SaveRoute);
            DeleteCommand = new RelayCommand(DeleteRoute);

            BuildGraph();
        }
        private void AddStep()
        {
            var newStep = new RouteStep { Name = "Новый сотрудник", Position = "Новая должность" };
            Steps.Add(newStep);
            SelectedStep = newStep;
            BuildGraph();
        }
        private void RemoveStep()
        {
            if (SelectedStep != null) 
            {
                Steps.Remove(SelectedStep);
                SelectedStep = null;
                BuildGraph();
            }
        }
        private void MoveUp()
        {
            int index = Steps.IndexOf(SelectedStep);
            if(index > 0)
            {
                Steps.Move(index, index - 1);
                BuildGraph();
            }
        }
        private void MoveDown() 
        {
            int index = Steps.IndexOf(SelectedStep); 
            if (index < Steps.Count -1 ) 
            {
                Steps.Move(index, index + 1);
                BuildGraph();
            }
        }
        private void BuildGraph()
        {
            var graph = new BidirectionalGraph<RouteNode, RouteEdge>();

            var nodes = Steps.Select(s => new RouteNode { Title = s.Name, Role = s.Position }).ToList();

            foreach (var node in nodes) 
            {
                graph.AddVertex(node);
            }
            for (int i = 0; i < nodes.Count - 1; i++) 
            {
                graph.AddEdge(new RouteEdge(nodes[i], nodes[i + 1]));
            }
            Graph = graph;
            OnPropertyChaneged(nameof(Graph));
        }
        private void SaveRoute()
        {

        }
        private void DeleteRoute()
        {

        }
    }
}
