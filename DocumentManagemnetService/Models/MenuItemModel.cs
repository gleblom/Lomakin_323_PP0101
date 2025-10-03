namespace DocumentManagementService.Models
{
    public class MenuItemModel
    {
        public string Title { get; }
        public string Icon { get; }
        public string PageKey { get; }

        public MenuItemModel(string title, string icon, string pageKey)    
        { 
            Title = title;    
            Icon = icon; 
            PageKey = pageKey; 
        }
    }
}
