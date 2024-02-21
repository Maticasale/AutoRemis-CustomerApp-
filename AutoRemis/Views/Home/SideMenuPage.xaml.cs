using Xamarin.Forms;
using Prism.Navigation;
using AutoRemis.Models;
using static AutoRemis.Helpers.AppStateManager;
using System.Linq;
using Xamarin.Essentials;
using Prism.Common;
using AutoRemis.Helpers;
using AutoRemis.Interfaces;

namespace AutoRemis.Views
{
    public partial class SideMenuPage : MasterDetailPage, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private readonly IGoogleManager _googleManager;


        private User user;

        private Trip trip;

        public SideMenuPage(INavigationService navigationService)
        {
            InitializeComponent();

            _navigationService = navigationService;
            _googleManager = DependencyService.Get<IGoogleManager>();

            LoadUI();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Parameters
            trip = parameters.GetValue<Trip>("Trip");
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        private void LoadUI()
        {
            //User and App Data
            user = GetUser();

            //General UI Settings
            user.FullName.Split(' ').ToList().ForEach(i => LblAvatar.Text += i[0].ToString());

            lblUserName.Text = user.FullName;
            lblPhoneNumber.Text = $"+54 9 {user.PhoneNumber}";
        }

        private void HelpClicked(object sender, System.EventArgs e)
        {
            var page = GetCurrentPage();
        }

        private async void LogOutClicked(object sender, System.EventArgs e)
        {
            BtnLogOut.IsEnabled = false;

            if (user.Init == InitType.Google) { }
                _googleManager.Logout();

            Preferences.Clear();
            await _navigationService.NavigateAsync("/NavigationPage/OnBoardingPage");

            IsPresented = false;
            BtnLogOut.IsEnabled = true;
        }

        private void HistoryClicked(object sender, System.EventArgs e)
        {

        }

        private void AboutClicked(object sender, System.EventArgs e)
        {

        }
    }
}