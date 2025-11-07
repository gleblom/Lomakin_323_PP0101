using DocumentManagementService.ViewModels;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class PublicDocumentsView : UserControl
    {
        public PublicDocumentsView()
        {
            InitializeComponent();
            DataContext = new PublicDocumentsViewModel();
        }
    }
}
