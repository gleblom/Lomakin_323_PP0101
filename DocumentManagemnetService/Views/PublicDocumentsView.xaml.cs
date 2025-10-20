using DocumentManagementService.ViewModels;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class PublicDocumentsView : UserControl
    {
        public PublicDocumentsView(Frame frame)
        {
            InitializeComponent();
            var navigationService = new NavigationService(frame);
            DataContext = new PublicDocumentsViewModel(navigationService);
        }
    }
}
