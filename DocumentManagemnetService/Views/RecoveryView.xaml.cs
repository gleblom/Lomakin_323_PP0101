using DocumentManagementService.ViewModels;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class RecoveryView : UserControl
    {
        public RecoveryView()
        {
            InitializeComponent();
            DataContext = new RecoveryViewModel();
        }
    }
}
