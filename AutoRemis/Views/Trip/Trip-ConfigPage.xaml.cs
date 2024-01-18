using AutoRemis.Helpers;
using AutoRemis.Models.Google;
using AutoRemis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Prism.Navigation;
using Prism.Ioc;
using Rg.Plugins.Popup.Extensions;
using static AutoRemis.Models.Google.GooglePlaceID;

namespace AutoRemis.Views
{
    public partial class Trip_ConfigPage : ContentPage, INavigatedAware
    {
        private readonly INavigationService _navigationService;

        private User user;

        ImageButton selectedPayMethod = null;
        private bool[] checkboxStates;

        private Trip trip;

        public Trip_ConfigPage(INavigationService navigationService)
        {
            InitializeComponent();

            _navigationService = navigationService;

            MessagingCenter.Subscribe<object, Trip>(this, "Trip", OnTripParamsChanged);
        }

        private void OnTripParamsChanged(object arg, Trip trip)
        {
            this.trip = trip;
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {            
            //Variables
            checkboxStates = new bool[10];
            trip = parameters.GetValue<Trip>("Trip");

            //User Data
            user = AppStateManager.GetUser();

            //General UI Settings

            //SearchBars Settings
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
            trip.discountCoupon = entryCoupon.Text;

            await _navigationService.NavigateAsync("/Trip_WaitingPage", new NavigationParameters { { "Trip", trip } });
        }
        public void OnNavigatedFrom(INavigationParameters parameters) {}

        private async void CalculateClicked(object sender, EventArgs e)
        {
            await _navigationService.NavigateAsync("Trip_ChangeMainParamsPopUp", new NavigationParameters { { "Trip", trip } });
        }
    }
}
