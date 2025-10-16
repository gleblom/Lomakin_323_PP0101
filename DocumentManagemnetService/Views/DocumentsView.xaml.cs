using DocumentManagementService.ViewModels;
using System.Windows.Controls;


namespace DocumentManagementService.Views
{
    /// <summary>
    /// Логика взаимодействия для DocumentsView.xaml
    /// </summary>
    public partial class DocumentsView : UserControl
    {
        public DocumentsView()
        {
            InitializeComponent();
            var navigationService = new NavigationService(DocumentsFrame);
            DataContext = new DocumentsViewModel(navigationService);
        }
    }
}
