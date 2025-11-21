using DocumentManagementService.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocumentManagementService.Views
{

    public partial class CompanyRegisterView : UserControl
    {
        public CompanyRegisterView()
        {
            InitializeComponent();
            DataContext = new CompanyRegisterViewModel();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
