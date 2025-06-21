namespace DocumentManagementService.Models
{
    public class MenuItemViewModel
    {
        public string Title { get; }
        public string Icon { get; }
        public string PageKey { get; }

        public MenuItemViewModel(string title, string icon, string pageKey)    
        { 
            Title = title;    
            Icon = icon; 
            PageKey = pageKey; 
        }
    }
}
