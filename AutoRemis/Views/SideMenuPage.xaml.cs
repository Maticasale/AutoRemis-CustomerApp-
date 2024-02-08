using Xamarin.Forms;
using Prism.Navigation;
using AutoRemis.Models;
using static AutoRemis.Helpers.AppStateManager;
using System.Linq;
using Xamarin.Essentials;
using Prism.Common;
using AutoRemis.Helpers;

namespace AutoRemis.Views
{
    public partial class SideMenuPage : MasterDetailPage, INavigatedAware
    {
        private readonly INavigationService _navigationService;

        private User user;
        private Trip trip;

        public SideMenuPage(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService; 
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {            
            //User Data
            user = GetUser();

            //Variables
            trip = parameters.GetValue<Trip>("Trip");

            //General UI Settings
            user.FullName.Split(' ').ToList().ForEach(i => LblAvatar.Text += i[0].ToString());

            lblUserName.Text = user.FullName;
            lblPhoneNumber.Text = $"+54 9 {user.PhoneNumber}";            
        }

        private void HelpClicked(object sender, System.EventArgs e)
        {
            var page = AppStateManager.GetCurrentPage();
        }

        private async void LogOutClicked(object sender, System.EventArgs e)
        {
            BtnLogOut.IsEnabled = false;

            Preferences.Clear();
            await _navigationService.NavigateAsync("/NavigationPage/OnBoardingPage");

            IsPresented = false;
            BtnLogOut.IsEnabled = true;
        }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}