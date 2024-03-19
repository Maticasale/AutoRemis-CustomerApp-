using AutoRemis.Helpers;
using AutoRemis.Models;
using Prism.Navigation;
using System;
using Xamarin.Forms;
using static AutoRemis.Helpers.LocationHelper;
using static AutoRemis.Helpers.AppStateManager;
using Xamarin.Forms.GoogleMaps;
using System.Threading.Tasks;

namespace AutoRemis.Views
{
    public partial class LoadingPage : ContentPage
    {
        private readonly INavigationService _navigationService;

        private User user;
        private AppSettings app;

        public LoadingPage(INavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();

            LoadUI();
        }

        private async void LoadUI()
        {
            Device.BeginInvokeOnMainThread(() => lblState.Text = "Cargando Datos");

            //User and App Data
            user = GetUser();
            app = GetAppInfo();

            var location = await GetLocation();

            switch (location.Status)
            {
                case LocationStatus.OK:
                    Device.BeginInvokeOnMainThread(() => lblState.Text = "Iniciando");

                    await Task.Delay(500);

                    user.lastKnownPosition = new Position(location.Location.Latitude, location.Location.Longitude);
                    //user.Status = UserStatus.Idle;
                    UpdateUser(user);

                    await _navigationService.NavigateAsync("/SideMenuPage", animated: true);
                    break;

                case LocationStatus.Unknown:
                    RiseErrorMsg("¡Aviso!", "No hemos podido determinar tu ubicación, por favor vuelve a intentarlo", 3, SoundHelper.SoundType.Alert);
                    break;


                case LocationStatus.Exception:
                    RiseErrorMsg("¡Error!", "El proceso de obtener tu ubicación Fallo. Verifica que la tengas activada en tu celular y vuelve a intentarlo.", 3, SoundHelper.SoundType.Error);
                    break;
            }

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
    }
}
