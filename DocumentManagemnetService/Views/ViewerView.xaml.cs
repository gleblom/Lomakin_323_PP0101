
using DocumentManagementService.ViewModels;


namespace DocumentManagementService.Views
{
    public partial class ViewerView : UserControl
    {

       
        public ViewerView()
        {
            InitializeComponent();
            INavigationService navigationService = new NavigationService(DocFrame);
            ViewerViewModel vm = new(navigationService);
            DataContext = vm;           
        }

    }
}
