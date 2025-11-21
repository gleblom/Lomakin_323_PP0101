using DocumentManagementService.ViewModels;
using DocumentManagementService.Views;
using DocumentManagemnetService.Views;
using System.Windows.Controls;


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
            routes["AllDocuments"] = () => new PublicDocumentsView("AllDocuments");
            routes["UploadDocument"] = () => new UploadDocumentView();
            routes["Incoming"] = () => new PublicDocumentsView("Incoming");
            routes["Account"] = () => new AccountView();
            routes["Profile"] = () => new ProfileView();
            routes["Viewer"] = () => new ViewerView();
            routes["Director"] = () => new DirectorView();
            routes["MyDocuments"] = () => new MyDocumentsView();
            routes["Approvement"] = () => new ApprovalView();
            routes["RouteView"] = () => new RoutesView();
            routes["AdminView"] = () => new AdminView();
            routes["ClerkView"] = () => new RoutesView();
            routes["View"] = () => new PdfViewerView();
            routes["Login"] = () => new LoginView();
            routes["SignUp"] = () => new CompanyRegisterView();
            routes["Recover"] = () => new RecoveryView();
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
