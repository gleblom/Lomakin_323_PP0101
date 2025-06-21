using DocumentManagementService.ViewModels;
using System.Windows.Controls;


namespace DocumentManagementService.Views
{
    /// <summary>
    /// Логика взаимодействия для RoutesView.xaml
    /// </summary>
    public partial class RoutesView : UserControl
    {
        public RoutesView()
        {
            InitializeComponent();
            DataContext = new RoutesViewModel();
        }
    }
}
