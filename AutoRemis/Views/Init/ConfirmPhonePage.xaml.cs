using AutoRemis.Helpers;
using AutoRemis.Models;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using static AutoRemis.Helpers.LocationManeger;
using static AutoRemis.Helpers.AppStateManager;

namespace AutoRemis.Views
{
    public partial class ConfirmPhonePage : ContentPage, INavigatedAware
    {
        User user;
        string token;
        bool isBussy;
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
            //Variables
            entryList = new List<Entry> { e1, e2, e3, e4 };
            frameList = new List<Frame> { f1, f2, f3, f4 };

            //User Data
            user = GetUser();
            token = parameters.GetValue<string>("ConfirmationToken");

            //General UI Settings
            lblName.Text = $"¡Hola {UI.CapitalizeSentence(user.FirstName)}!";
            lblPhone.Text = $"+54 9 {user.PhoneNumber}";
            btnResend.IsEnabled = false;
            EnableResendTokenButton();
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

        private void GoBack(object sender, EventArgs e) => _navigationService.GoBackAsync();

        private void F1C(object sender, TextChangedEventArgs e)
        {
            if (!isBussy)
            {
                e2.Focus();
                CheckToken();
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

                    RiseErrorMsg("¡Error!", "El codigo de verificacion es incorrecto, vuelve a intentarlo", 3, SoundTools.SoundType.Error);
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

                foreach (var i in frameList)
                    i.BorderColor = Color.White;
                SoundTools.StopCurrentSound();
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}