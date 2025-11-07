using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class RoutesView : UserControl
    {
        public RoutesView()
        {
            InitializeComponent();
            RoutesViewModel vm = new();
            DataContext = vm;
            vm.ShowAction ??= new Action(ShowButtons);

        }
        public void ShowButtons()
        {
            //CreateRoute.Visibility = Visibility.Visible;
            //EditRoute.Visibility = Visibility.Visible;
        }
    }
}
