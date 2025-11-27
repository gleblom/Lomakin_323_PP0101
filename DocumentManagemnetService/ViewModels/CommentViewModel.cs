using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentManagementService.ViewModels
{
    public class CommentViewModel: BaseViewModel
    {
        public ICommand CancelCommand { get; }
        public ICommand AddCommentCommand { get; }
        public Action CancelAction { get; set; }
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    OnPropertyChanged(nameof(isEnabled));
                }
            }
        }
        public Action<string> AddCommentAction { get; set; }
        private string comment;
        public string Comment
        {
            get { return comment; }
            set 
            { 
                comment = value; 
                OnPropertyChanged();
            }
        }
        public void ExecuteComment()
        {
            IsEnabled = false;
            AddCommentAction?.Invoke(comment);
            CancelAction();
            IsEnabled = true;
        }
        public CommentViewModel()
        {
            CancelCommand = new RelayCommand(CancelAction);
            AddCommentCommand = new RelayCommand(ExecuteComment, 
                obj => comment != string.Empty);
        }

    }
}
