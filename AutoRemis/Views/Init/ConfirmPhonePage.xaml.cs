using AutoRemis.Helpers;
using AutoRemis.Models;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using static AutoRemis.Helpers.LocationHelper;
using static AutoRemis.Helpers.AppStateManager;

namespace AutoRemis.Views
{
    public partial class ConfirmPhonePage : ContentPage, INavigatedAware
    {
        private User user;

        private string token;
        private bool isBussy;
        private InitType init;
        private List<Entry> entryList;
        private List<Frame> frameList;

        private readonly INavigationService _navigationService;
        public ConfirmPhonePage(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //User Data
            user = GetUser();

            //Variables
            entryList = new List<Entry> { e1, e2, e3, e4 };
            frameList = new List<Frame> { f1, f2, f3, f4 };
            init = parameters.GetValue<InitType>("LoginType");
            token = parameters.GetValue<string>("ConfirmationToken");

            //General UI Settings
            EnableResendTokenButton();
            btnResend.IsEnabled = false;
            lblPhone.Text = $"+54 9 {user.PhoneNumber}";
            lblName.Text = $"¡Hola {UIHelper.CapitalizeSentence(user.FirstName)}!";
            Device.BeginInvokeOnMainThread(() => e1.Focus());
        }

        private void EntryFocused(object sender, FocusEventArgs e)
        {
            foreach (var i in frameList)
                i.BorderColor = Color.White;

            var entry = (Entry)sender;

            Element parent = entry.Parent;

            while (parent != null && !(parent is Frame))
                parent = parent.Parent;

            if (parent is Frame frame)
                frame.BorderColor = Color.Green;
        }

        private void StkTapped(object sender, EventArgs e)
        {
            foreach (var i in entryList)
                i.Unfocus();
            foreach (var i in frameList)
                i.BorderColor = Color.White;
        }

        private void EnableResendTokenButton()
        {
            var i = 60;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                i--;
                lblTimeLeft.Text = i.ToString();

                if (i == 0)
                {
                    Device.BeginInvokeOnMainThread(() => 
                    {
                        btnResend.TextColor = Color.Green;
                        btnResend.IsEnabled = true;
                    });
                }
                return i != 0;
            });
        }
        protected override bool OnBackButtonPressed() => true;
        private void GoBackClicked(object sender, EventArgs e) => _navigationService.GoBackAsync(new NavigationParameters { { "LoginType", init } });


        private void F1C(object sender, TextChangedEventArgs e)
        {
            if (!isBussy)
            {
                CheckToken();
                e2.Focus();
            }
        }
        private void F2C(object sender, TextChangedEventArgs e)
        {
            if (!isBussy)
            {
                e3.Focus();
                CheckToken();
            }
        }
        private void F3C(object sender, TextChangedEventArgs e)
        {
            if (!isBussy)
            {
                e4.Focus();
                CheckToken();
            }
        }
        private void F4C(object sender, TextChangedEventArgs e)
        {
            if (!isBussy)
            {
                CheckToken();
                e4.Unfocus();
            }
        }
        private async void CheckToken()
        {
            if (!string.IsNullOrWhiteSpace(e1.Text) && !string.IsNullOrWhiteSpace(e2.Text) && !string.IsNullOrWhiteSpace(e3.Text) && !string.IsNullOrWhiteSpace(e4.Text))
            {
                isBussy = true;

                foreach (var i in entryList)
                    i.IsEnabled = false;

                if ((e1.Text+e2.Text+e3.Text+e4.Text) == token)
                {
                    foreach (var i in frameList)
                        i.BorderColor = Color.Green;

                    stkResend.IsVisible = false;
                    stkState.IsVisible = true;
                    stateIndicator.IsRunning = true;

                    Device.BeginInvokeOnMainThread(() => lblState.Text = "Obteniendo su ubicación");

                    if (await GetLocation(UserStatus.Idle) != LocationStatus.OK) return;

                    Device.BeginInvokeOnMainThread(() => lblState.Text = "Iniciando");
                    await Task.Delay(1000);

                    await _navigationService.NavigateAsync("/SideMenuPage", animated: true);
                }
                else
                {
                    foreach (var i in frameList)
                        i.BorderColor = Color.Red;
                    foreach (var i in entryList)
                        i.Text = string.Empty;

                    RiseErrorMsg("¡Error!", "El codigo de verificacion es incorrecto, vuelve a intentarlo", 3, SoundHelper.SoundType.Error);
                }

                foreach (var i in entryList)
                    i.IsEnabled = true;

                isBussy = false;
            }
        }
        private void Resend(object sender, EventArgs e)
        {
            btnResend.TextColor = Color.Green;
            btnResend.IsEnabled = true;
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

                foreach (var i in frameList)
                    i.BorderColor = Color.White;
                SoundHelper.StopCurrentSound();
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}