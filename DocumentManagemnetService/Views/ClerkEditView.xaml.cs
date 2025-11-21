using System.Windows;
using System.Windows.Input;

namespace DocumentManagementService.Views
{

    public partial class ClerkEditView : Window
    {
        public ClerkEditView()
        {
            InitializeComponent();
        }
        public void DisableEmailBox()
        {
            EmailBox.IsEnabled = false;
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
