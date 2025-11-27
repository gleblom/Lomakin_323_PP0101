using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{

    public partial class ApprovalView : UserControl
    {
        public ApprovalView()
        {
            InitializeComponent();
            ApprovalViewModel vm = new(App.notifications);
            DataContext = vm;
        }

    }
}
