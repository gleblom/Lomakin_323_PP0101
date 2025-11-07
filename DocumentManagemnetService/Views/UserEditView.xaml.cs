using DocumentManagementService.ViewModels;
using System.Windows;



namespace DocumentManagementService.Views
{

    public partial class UserEditView : Window
    {
        public UserEditView()
        {
            InitializeComponent();
            UserEditViewModel vm = new();
            DataContext = vm;
        }
    }
}
