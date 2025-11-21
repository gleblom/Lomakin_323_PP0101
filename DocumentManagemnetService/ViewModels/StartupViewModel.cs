namespace DocumentManagementService.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        private readonly INavigationService navigationService;
        public StartupViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            navigationService.Navigate("Login");
        }
    }
}
