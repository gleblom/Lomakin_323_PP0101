using DocumentManagementService.ViewModels;
using DocumentManagemnetService;
using System.Windows.Controls;
using System.Windows;

namespace DocumentManagementService.Views
{

    public partial class RoutesView : UserControl
    {
        public RoutesView()
        {
            InitializeComponent();
            RoutesViewModel vm = new();
            DataContext = vm;


            if (App.CurrentUser.RoleId == 2)
            {
                NewRoute.Visibility = Visibility.Visible;
                Cancel.Visibility = Visibility.Collapsed;
                OnApprove.Visibility = Visibility.Collapsed;
            }

        }
    }
}
