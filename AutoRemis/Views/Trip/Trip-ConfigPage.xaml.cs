using AutoRemis.Helpers;
using AutoRemis.Models;
using System;
using System.Linq;
using Xamarin.Forms;
using Prism.Navigation;
using System.Threading.Tasks;

namespace AutoRemis.Views
{
    public partial class Trip_ConfigPage : ContentPage, INavigatedAware
    {
        private readonly INavigationService _navigationService;

        private User user;
        private Trip trip;

        private ImageButton selectedPayMethod = null;
        private bool[] checkboxStates;


        public Trip_ConfigPage(INavigationService navigationService)
        {
            InitializeComponent();

            _navigationService = navigationService;

            MessagingCenter.Subscribe<object, Trip>(this, "Trip", OnTripParamsChanged);
        }

        private void OnTripParamsChanged(object arg, Trip trip) => this.trip = trip;

        public void OnNavigatedTo(INavigationParameters parameters)
        {            
            //User Data
            user = AppStateManager.GetUser();

            //Variables
            checkboxStates = new bool[10];
            trip = parameters.GetValue<Trip>("Trip");
        }

        private void ObsClicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;

            Device.BeginInvokeOnMainThread(() =>
            {
                // Obtener el índice del botón (suponiendo que los botones están en orden)
                int index = int.Parse(button.ClassId);

                // Cambiar el estado de la checkbox
                checkboxStates[index] = !checkboxStates[index];

                // Cambiar la imagen dependiendo del estado actual
                button.Source = checkboxStates[index] ? "iconFilledChk1.png" : "iconEmptyChk1.png";

                // Convertir el arreglo de booleanos a un string
                string result = string.Join("", checkboxStates.Select(b => b ? "1" : "0"));
                trip.observation = $"&{result}";
            });
        }

        private void PayMethodClicked(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (selectedPayMethod != null)
                    selectedPayMethod.Source = "iconEmptyChk1.png";

                button.Source = "iconFilledChk1.png";

                selectedPayMethod = button;
            });
        }

        private async void StartTrip(object sender, EventArgs e)
        {
            stateIndicator.IsRunning = true;
            lblBtnStartTrip.IsEnabled = false;
            lblBtnStartTrip.IsVisible = false;

            if (selectedPayMethod != null)
            {
                trip.discountCoupon = entryCoupon.Text;
                trip.paymentMethod = selectedPayMethod.ClassId;
                user.TripInfo = trip;
                await _navigationService.NavigateAsync("/Trip_WaitingPage", new NavigationParameters { { "Trip", trip } });

            }
            else
                RiseErrorMsg("Advertencia", "Necesitamos que selecciones un metodo de pago por favor", 2, SoundHelper.SoundType.Alert);

            stateIndicator.IsRunning = false;
            lblBtnStartTrip.IsEnabled = true;
            lblBtnStartTrip.IsVisible = true;
        }
        private async void CalculateClicked(object sender, EventArgs e) => await _navigationService.NavigateAsync("Trip_ChangeMainParamsPopUp", new NavigationParameters { { "Trip", trip } });

        public void OnNavigatedFrom(INavigationParameters parameters) {}

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
