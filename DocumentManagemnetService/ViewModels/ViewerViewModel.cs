using DocumentManagementService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DocumentManagementService.ViewModels
{
    class ViewerViewModel: BaseViewModel
    {
        public ObservableCollection<MenuItemModel> DocumentItems { get; }
        public ViewerViewModel()
        {
            DocumentItems =
                [
                    new("Просмотр", "Image", "View"),
                    new("Связанные документы",  "LinkVariant", "Link"),
                    new("Комментарии", "CommentOutline", "Comments"),
                    new("Прошлые версии", "ClockOutline", "Versions"),
                    new("Согласование", "Check", "Approvement")
                ];
        }
    }
}
