using Xamarin.Forms;
using Prism.Navigation;
using AutoRemis.Models;
using AutoRemis.Helpers;
using System.Linq;

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
            user = AppStateManager.GetUser();

            //Variables
            trip = parameters.GetValue<Trip>("Trip");

            //General UI Settings
            user.FullName.Split(' ').ToList().ForEach(i => LblAvatar.Text += i[0].ToString());

            lblUserName.Text = user.FullName;
            
        }

        private void HelpClicked(object sender, System.EventArgs e)
        {

        }

        private void LogOutClicked(object sender, System.EventArgs e)
        {

        }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}