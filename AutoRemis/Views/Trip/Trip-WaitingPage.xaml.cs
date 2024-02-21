using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Services;
using FFImageLoading.Svg.Forms;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace AutoRemis.Views
{
    public partial class Trip_WaitingPage : ContentPage, INavigatedAware
    {
        private readonly INavigationService _navigationService;

        private User user;

        private Trip trip;

        private Pin driverPin;


        public Trip_WaitingPage(INavigationService navigationService)
        {
            InitializeComponent();            
            
            _navigationService = navigationService;

            MessagingCenter.Subscribe<object>(this, "TripAccepted", (sender) => TripAccepted());
        }

        public void OnNavigatedTo(INavigationParameters parameters) 
        {
            //Variables
            trip = parameters.GetValue<Trip>("Trip");

            LoadUI();
        }
        private void LoadUI()
        {
            //User and App Data
            user = AppStateManager.GetUser();

            //General UI Settings
            map.IsEnabled = true;
            map.UiSettings.ZoomControlsEnabled = false;
            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(user.lastKnownPosition, 14d);

            Device.BeginInvokeOnMainThread(() =>
            {
                bx1.IsVisible = true;
                bx2.IsVisible = true;

                gif.Children.Add(new SvgCachedImage { Aspect = Aspect.AspectFit, Source = "gifRadar.gif", VerticalOptions = LayoutOptions.Center });

                map.Pins.Add(new Pin()
                {
                    Type = PinType.SearchResult,
                    Position = user.lastKnownPosition,
                    Label = user.FirstName,
                    Icon = BitmapDescriptorFactory.FromBundle("pinUser1.png")
                });

                map.MoveToRegion(MapSpan.FromCenterAndRadius(user.lastKnownPosition, Distance.FromKilometers(14d)));
            });
        }

        protected override bool OnBackButtonPressed() => true;

        private void CancelClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                lblState.Text = "¡PEDIDO RECEPCIONADO!";
                bx1.IsVisible = false;
                bx2.IsVisible = false;
                gif.IsVisible = false;
                btnCancel.IsVisible = false;
                ContainmentBox.IsVisible = false;

                DriverInfoBox.IsVisible = true;
                btnCancel1.IsVisible = true;
                btnCall.IsVisible = true;
                user.FullName.Split(' ').ToList().ForEach(i => LblAvatar.Text += i[0].ToString());

                await _navigationService.NavigateAsync("Trip_StateInfoPopUp", new NavigationParameters { { "Msg", "Su pedido ha sido recepcionado exitosamente" } });
            });
        }

        public void TripAccepted()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                TrackPath();

                lblState.Text = "¡TU REMIS YA ESTÁ EN CAMINO!";
                map.Pins.Add(new Pin()
                {
                    Type = PinType.SearchResult,
                    Position = new Position(Convert.ToDouble(trip.lat_destination), Convert.ToDouble(trip.lng_destination)),
                    Label = "Jose Luis",
                    Icon = BitmapDescriptorFactory.FromBundle("pinDriver.png")
                    //Rotation = bearing
                });
                map.IsEnabled = false;


            });
        }         
        private void CallClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                btnBoard.IsVisible = true;
            });
        }
        private void BoardClicked(object sender, EventArgs e)
        {

        }
        async void TrackPath()
        {
            var pathcontent = await LoadRoute();
            map.Polylines.Clear();

            try
            {
                var polyline = new Polyline();
                polyline.StrokeColor = Color.FromHex("6d9ace");
                polyline.StrokeWidth = 3;

                foreach (var p in pathcontent)
                    polyline.Positions.Add(p);

                map.Polylines.Add(polyline);

                // Define la distancia mínima a mostrar en el mapa
                const double distanciaMinimaEnKm = 0.2;

                // Calcula la distancia real entre los puntos
                var distanciaEnKm = Location.CalculateDistance(
                    user.lastKnownPosition.Latitude,
                    user.lastKnownPosition.Longitude,
                    double.Parse(trip.lat_destination),
                    double.Parse(trip.lng_destination),
                    DistanceUnits.Kilometers);

                // Calcula el nivel de zoom necesario para mostrar la distancia mínima
                var radioEnKm = Math.Max(distanciaEnKm / 2, distanciaMinimaEnKm);
                var zoom = Math.Log(40075 / (256 * radioEnKm)) / Math.Log(2);

                // Calcula la posición del centro del mapa
                var latitudMedia = (user.lastKnownPosition.Latitude + double.Parse(trip.lat_destination)) / 2;
                var longitudMedia = (user.lastKnownPosition.Longitude + double.Parse(trip.lng_destination)) / 2;

                // Mueve el mapa al centro y establece el zoom adecuado
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitudMedia, longitudMedia), Distance.FromKilometers(radioEnKm + 0.2)));
            }
            catch (Exception) { }
        }

        internal async Task<List<Position>> LoadRoute()
        {
            try
            {
                var googleDirection = await Places.GetDirections(user.lastKnownPosition.Latitude.ToString(), user.lastKnownPosition.Longitude.ToString(), trip.lat_destination, trip.lng_destination, "AIzaSyDxKLNaQ8S-k2D7MY0dvxMbRYWtuRQV0PI");
                if (googleDirection.Routes != null && googleDirection.Routes.Count > 0)
                {
                    var positions = (Enumerable.ToList(PolylineHelper.Decode(googleDirection.Routes.First().OverviewPolyline.Points)));
                    return positions;
                }
                else return null;
            }
            catch (Exception) { }
            return null;
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}