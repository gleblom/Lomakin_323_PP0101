using DocumentManagementService.Views;
using System.Windows.Controls;

namespace DocumentManagementService
{
    public class NavigationService: INavigationService
    {
        private readonly Frame mainFraim;
        private readonly Dictionary<string, Func<UserControl>> routes = [];
        public NavigationService(Frame frame) 
        {
            mainFraim = frame;

            routes["Documents"] = () => new DocumentsView();
            routes["AllDocuments"] = () => new PublicDocumentsView();
            routes["UploadDocument"] = () => new UploadDocumentView();
            routes["Routes"] = () => new RoutesView();
        }

        public void Navigate(string pageKey)
        {
            if (routes.TryGetValue(pageKey, out var route)) {
                mainFraim.Navigate(route());
            }
        }
    }
}
