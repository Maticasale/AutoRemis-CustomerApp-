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
using Prism.Ioc;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms.PlatformConfiguration;

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

        public void OnNavigatedFrom(INavigationParameters parameters) { }


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

        private void BackgroundTapped(object sender, EventArgs e)
        {
            if (isBussy)
                return;

            foreach (var i in entryList)
                i.Unfocus();

            FrameColor(Color.White);
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
            if (string.IsNullOrWhiteSpace(e1.Text) || string.IsNullOrWhiteSpace(e2.Text) || string.IsNullOrWhiteSpace(e3.Text) || string.IsNullOrWhiteSpace(e4.Text))
                return;

            isBussy = true;

            foreach (var i in entryList)
                i.IsEnabled = false;

            foreach (var i in entryList)
                i.Unfocus();

            if ((e1.Text+e2.Text+e3.Text+e4.Text) == token)
            {
                FrameColor(Color.Green);

                StartLogin();                
            }
            else
            {
                FrameColor(Color.Red);

                foreach (var i in entryList)
                    i.Text = string.Empty;

                RiseErrorMsg("¡Error!", "El codigo de verificacion es incorrecto, vuelve a intentarlo", 3, SoundHelper.SoundType.Error);

                await Task.Delay(3100);

                FrameColor(Color.White);

                e1.Focus();

                background.IsEnabled = true;

                isBussy = false;

                foreach (var i in entryList)
                    i.IsEnabled = true;
            }

        }

        public async void StartLogin()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status == PermissionStatus.Granted)
                    FinishLogin();
                else
                {
                    RiseErrorMsg("¡Aviso!", "Para usar esta aplicación debes permitir el acceso a la ubicacion, para eso ve a la configuración de la misma y activala por favor", 4, SoundHelper.SoundType.Alert);
                    isLoading(false);
                }
            }
            else
                FinishLogin();
        }
        

        public async void FinishLogin()
        {
            isLoading(true);

            Device.BeginInvokeOnMainThread(() => lblState.Text = "Obteniendo su ubicación");

            var location = await GetLocation();
            switch (location.Status)
            {
                case LocationStatus.OK:
                    
                    user.lastKnownPosition = new Position(location.Location.Latitude, location.Location.Longitude);
                    user.Status = UserStatus.Idle;
                    user.TokenFCM = Preferences.Get("FirebaseToken", "");
                    UpdateUser(user);

                    Device.BeginInvokeOnMainThread(() => lblState.Text = "Iniciando");

                    await Task.Delay(500);

                    await _navigationService.NavigateAsync("/SideMenuPage", animated: true);

                    break;

                case LocationStatus.Unknown:

                    RiseErrorMsg("¡Aviso!", "No hemos podido determinar tu ubicación, por favor vuelve a intentarlo", 3, SoundHelper.SoundType.Alert);
                    foreach (var i in entryList)
                        i.IsEnabled = true;

                    isBussy = false;
                    isLoading(false);

                    break;

                case LocationStatus.Exception:

                    RiseErrorMsg("¡Error!", "El proceso de obtener tu ubicación Fallo. Verifica que la tengas activada en tu celular y vuelve a intentarlo.", 3, SoundHelper.SoundType.Error);
                    foreach (var i in entryList)
                        i.IsEnabled = true;

                    isBussy = false;
                    isLoading(false);

                    break;
            }
        }

        public void isLoading(bool isLoading)
        {
            stkResend.IsVisible = false;
            stkState.IsVisible = isLoading;
            stateIndicator.IsRunning = isLoading;
            btnRetryLocation.IsVisible = !isLoading;
        }
        private void RetryLocation(object sender, EventArgs e) => StartLogin();

        private void Resend(object sender, EventArgs e)
        {
            //TODO: Servicio de reenviar Token
            btnResend.TextColor = Color.Green;
            btnResend.IsEnabled = true;
        }

        private void RiseErrorMsg(string title, string msg, int time, SoundHelper.SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                background.IsEnabled = false;

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

                background.IsEnabled = true;

                SoundHelper.StopCurrentSound();
            });
        }

        public void FrameColor(Color color) => Device.BeginInvokeOnMainThread(() => { foreach (var i in frameList) i.BorderColor = color; });                                
    }
}