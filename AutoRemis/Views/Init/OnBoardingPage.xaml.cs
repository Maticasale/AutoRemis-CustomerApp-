using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Models.Google;
using Prism.Navigation;
using System.Threading.Tasks;
using System;
using Xamarin.Forms;

namespace AutoRemis.Views
{
    public partial class OnBoardingPage : ContentPage
    {
        private readonly INavigationService _navigationService;
        private readonly IGoogleManager _googleManager;
        public OnBoardingPage(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            _googleManager = DependencyService.Get<IGoogleManager>();
        }

        private void CheckUserLoggedIn() => _googleManager.Login(OnLoginComplete);
        private async void OnLoginComplete(GoogleUser googleUser, string message)
        {
            if (googleUser != null)
            {
                await _navigationService.NavigateAsync("/RegisterPage", new NavigationParameters{{"LoginType", InitType.Google}, { "GoogleUser", googleUser} });
            }
            else
                RiseErrorMsg("¡Error!", "Ocurrio una falla tratando de iniciar el servicio de Google, por favor vuelva a intetnar", 3, SoundTools.SoundType.Error);
        }

        private void GoogleClicked(object sender, EventArgs e) => _googleManager.Login(OnLoginComplete);

        private void RiseErrorMsg(string title, string msg, int time, SoundTools.SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

                Title.Text = title;
                Msg.Text = msg;

                switch (type)
                {
                    case SoundTools.SoundType.Error:
                        imgItem.Source = "iconError.png";
                        CancellBox.BorderColor = Color.FromHex("#ff355b");
                        CancellBox.BackgroundColor = Color.FromHex("#fffbfc");
                        Title.TextColor = Color.FromHex("#ff355b");
                        SoundTools.PlaySound(SoundTools.SoundType.Alert);
                        break;
                    case SoundTools.SoundType.Alert:
                        imgItem.Source = "iconWarning.png";
                        CancellBox.BorderColor = Color.FromHex("#FFC021");
                        CancellBox.BackgroundColor = Color.FromHex("#fffefb");
                        Title.TextColor = Color.FromHex("#FFC021");
                        SoundTools.PlaySound(SoundTools.SoundType.Alert);
                        break;
                    case SoundTools.SoundType.Success:
                        imgItem.Source = "iconSuccess.png";
                        CancellBox.BorderColor = Color.FromHex("#47D764");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfefc");
                        Title.TextColor = Color.FromHex("#47D764");
                        SoundTools.PlaySound(SoundTools.SoundType.Success);
                        break;
                    case SoundTools.SoundType.Message:
                        imgItem.Source = "iconInfo.png";
                        CancellBox.BorderColor = Color.FromHex("#2F86EB");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfdff");
                        Title.TextColor = Color.FromHex("#2F86EB");
                        SoundTools.PlaySound(SoundTools.SoundType.Message);
                        break;
                }

                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

                SoundTools.StopCurrentSound();
            });
        }

        private void PhoneClicked(object sender, EventArgs e)
        {
            _googleManager.Logout();
        }
    }
}
