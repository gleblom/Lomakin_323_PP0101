using DocumentManagementService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DocumentManagementService.Views
{
    /// <summary>
    /// Логика взаимодействия для ApprovalView.xaml
    /// </summary>
    public partial class ApprovalView : UserControl
    {
        public ApprovalView()
        {
            InitializeComponent();
            DataContext = new ApprovalViewModel();
        }
    }
}
