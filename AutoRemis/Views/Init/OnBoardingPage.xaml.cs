using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Models.Google;
using Prism.Navigation;
using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using AutoRemis.Interfaces;

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
        private async void OnLoginComplete(GoogleUser googleUser, string message)
        {
            IsBusy(true);
            btnGoogle.IsEnabled = false;
            if (googleUser != null)
                await _navigationService.NavigateAsync("RegisterPage", new NavigationParameters{{"LoginType", InitType.Google}, { "GoogleUser", googleUser} });
            //else
            //    RiseErrorMsg("¡Error!", "Ocurrio una falla tratando de iniciar el servicio de Google, por favor vuelva a intetnar", 3, SoundHelper.SoundType.Error);
            IsBusy(false);
        }

        private void GoogleClicked(object sender, EventArgs e) => _googleManager.Login(OnLoginComplete);

        private async void PhoneClicked(object sender, EventArgs e)
        {
            IsBusy(true);
            await _navigationService.NavigateAsync("RegisterPage", new NavigationParameters { { "LoginType", InitType.PhoneNumber } });
            IsBusy(false);
        }

        private void RiseErrorMsg(string title, string msg, int time, SoundHelper.SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

                Title.Text = title;
                Msg.Text = msg;

                switch (type)
                {
                    case SoundHelper.SoundType.Error:
                        imgItem.Source = "iconError.png";
                        CancellBox.BorderColor = Color.FromHex("#ff355b");
                        CancellBox.BackgroundColor = Color.FromHex("#fffbfc");
                        Title.TextColor = Color.FromHex("#ff355b");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Alert:
                        imgItem.Source = "iconWarning.png";
                        CancellBox.BorderColor = Color.FromHex("#FFC021");
                        CancellBox.BackgroundColor = Color.FromHex("#fffefb");
                        Title.TextColor = Color.FromHex("#FFC021");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Success:
                        imgItem.Source = "iconSuccess.png";
                        CancellBox.BorderColor = Color.FromHex("#47D764");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfefc");
                        Title.TextColor = Color.FromHex("#47D764");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Success);
                        break;
                    case SoundHelper.SoundType.Message:
                        imgItem.Source = "iconInfo.png";
                        CancellBox.BorderColor = Color.FromHex("#2F86EB");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfdff");
                        Title.TextColor = Color.FromHex("#2F86EB");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Message);
                        break;
                }

                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

                SoundHelper.StopCurrentSound();
            });
        }

        private new void IsBusy(bool value)
        {
            btnGoogle.IsEnabled = !value;
            //btnMail.IsEnabled = !value;
            btnPhone.IsEnabled = !value;
        }
    }
}