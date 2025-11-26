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
            AddCommentAction?.Invoke(comment);
            CancelAction();
        }
        public CommentViewModel()
        {
            CancelCommand = new RelayCommand(CancelAction);
            AddCommentCommand = new RelayCommand(ExecuteComment, 
                obj => comment != string.Empty);
        }

    }
}
