using DocumentManagementService.ViewModels;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class RoutesView : UserControl
    {
        public RoutesView(Frame frame)
        {
            InitializeComponent();
            INavigationService navigationService = new NavigationService(frame);
            RoutesViewModel vm = new(navigationService);
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
