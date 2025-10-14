using DocumentManagementService.ViewModels;
using DocumentManagementService.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DocumentManagementService
{
    public class NavigationService: INavigationService // Класс для навигации внутри Frame
    {
        private readonly Frame mainFraim;
        private readonly Dictionary<string, Func<UserControl>> routes = [];
        public NavigationService(Frame frame) 
        {
            mainFraim = frame;

            routes["Documents"] = () => new DocumentsView();
            routes["AllDocuments"] = () => new PublicDocumentsView(mainFraim);
            routes["UploadDocument"] = () => new UploadDocumentView();
            routes["Incoming"] = () => new ApprovalView();
            routes["Account"] = () => new AccountView();
            routes["Profile"] = () => new ProfileView();
            routes["Viewer"] = () => new ViewerView();
            routes["Director"] = () => new DirectorView();
        }

        public void Navigate(string pageKey)
        {
            //Переход на страницу происходит по ключу
            if (routes.TryGetValue(pageKey, out var route)) {
                mainFraim.Navigate(route());
            }
        }
    }
}
