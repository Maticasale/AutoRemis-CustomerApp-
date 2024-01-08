using AutoRemis.Helpers;
using AutoRemis.Models.Google;
using Xamarin.Forms;

namespace AutoRemis.Views
{
    public partial class OnBoardingPage : ContentPage
    {
        private bool isPasswordVisible = true;
        private readonly IGoogleManager _googleManager;
        public OnBoardingPage()
        {
            InitializeComponent();
            _googleManager = DependencyService.Get<IGoogleManager>();
        }

        private void CheckUserLoggedIn() => _googleManager.Login(OnLoginComplete);
        private void OnLoginComplete(GoogleUser googleUser, string message)
        {
            var user = AppStateManager.GetUser();

            if (googleUser != null)
            {
                user.Name = googleUser.Name;

            }
            else
                DisplayAlert("Message", message, "Ok");

            AppStateManager.UpdateUser(user);

        }

        private void GoogleClicked(object sender, System.EventArgs e) => _googleManager.Login(OnLoginComplete);

        private void PhoneClicked(object sender, System.EventArgs e)
        {
            _googleManager.Logout();
        }
    }
}
