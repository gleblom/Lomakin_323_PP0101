using DocumentManagementService.ViewModels;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class DirectorView : UserControl
    {
        public DirectorView()
        {
            InitializeComponent();
            DataContext = new DirectorViewModel();
        }
    }
}
