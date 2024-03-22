using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Models.Services;
using AutoRemis.Services;
using DryIoc;
using FFImageLoading.Svg.Forms;
using Polly;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace AutoRemis.Views
{
    public partial class Trip_WaitingPage : ContentPage, INavigatedAware
    {
        private readonly INavigationService _navigationService;

        private Timer timer;
        private readonly object lockObject = new object();
        private bool isRunning = false, nearCarShown = false;
        private int state = -1;

        private User user;
        private Trip trip;
        private Pin driverPin;
        Location orgLoc, dstLoc, driverLoc, userLoc;


        public Trip_WaitingPage(INavigationService navigationService)
        {
            InitializeComponent();            
            
            _navigationService = navigationService;

            MessagingCenter.Subscribe<object, int>(this, "TripAccepted", (sender, args) => TripAccepted(args));
        }

        public void OnNavigatedTo(INavigationParameters parameters) 
        {
            //Variables
            trip = parameters.GetValue<Trip>("Trip");

            orgLoc = new Location() { Latitude = double.Parse(trip.lat_origin), Longitude = double.Parse(trip.lng_origin) };
            dstLoc = new Location() { Latitude = double.Parse(trip.lat_destination), Longitude = double.Parse(trip.lng_destination) };
            userLoc = new Location() { Latitude = double.Parse(trip.lat_device), Longitude = double.Parse(trip.lng_device) };

            LoadUI();
        }
        private async void LoadUI()
        {
            //User and App Data
            user = AppStateManager.GetUser();

            //General UI Settings
            map.IsEnabled = true;
            map.UiSettings.ZoomControlsEnabled = false;
            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(user.lastKnownPosition, 14d);

            AdjustCamera(orgLoc, dstLoc);

            await DrawRoute(orgLoc, dstLoc, "000");

            Device.BeginInvokeOnMainThread(() =>
            {
                map.Pins.Add(new Pin()
                {
                    Type = PinType.SearchResult,
                    Position = new Position(orgLoc.Latitude, orgLoc.Longitude),
                    Label = "Origen",
                    Icon = BitmapDescriptorFactory.FromBundle("pinDot.png"),
                });

                map.Pins.Add(new Pin()
                {
                    Type = PinType.SearchResult,
                    Position = new Position(dstLoc.Latitude, dstLoc.Longitude),
                    Label = "Destino",
                    Icon = BitmapDescriptorFactory.FromBundle("pinLoc.png")
                });
            });

            timer = new Timer(async (state) => await StartTimer(), null, TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
        }

        private async Task StartTimer()
        {
            var result = await TripService.GetTripState(new TripState() { id_viaje = trip.id_viaje, phone = trip.phone });

            Debug.WriteLine($"Iteracion: status {result.status}");
            switch (result.status)
            {
                case "0":
                    if (state == 0)
                        return;
                    state = 0;

                    Device.BeginInvokeOnMainThread(() =>
                    {

                        bx1.IsVisible = true;
                        bx2.IsVisible = true;
                        gif.Children.Add(new SvgCachedImage { Aspect = Aspect.AspectFit, Source = "gifRadar.gif", VerticalOptions = LayoutOptions.Center });
                    });
                    break;

                case "1":
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        driverLoc = new Location() { Latitude = double.Parse(result.lat), Longitude = double.Parse(result.lng) };

                        if (state != 1)
                        {
                            state = 1;

                            bx1.IsVisible = false;
                            bx2.IsVisible = false;
                            gif.IsVisible = false;
                            btnCancel.IsVisible = false;
                            ContainmentBox.IsVisible = false;

                            DriverName.Text = result.driver;
                            VehicleBrand.Text = result.car.model;
                            DriverID.Text = result.car.movil;
                            Domain.Text = result.car.domain;
                            Price.Text = trip.price;
  
                            DriverInfoBox.IsVisible = true;
                            btnCancel1.IsVisible = true;
                            btnCall.IsVisible = true;
                            map.IsEnabled = false;
                            lblState.Text = "¡PEDIDO RECEPCIONADO!";


                            driverPin = new Pin()
                            {
                                Type = PinType.SearchResult,                                    
                                Label = result.driver,
                                Icon = BitmapDescriptorFactory.FromBundle("pinDriver.png")
                            };

                            await _navigationService.NavigateAsync("Trip_StateInfoPopUp", new NavigationParameters { { "Msg", "Su pedido ha sido recepcionado exitosamente" }, { "Type", 1 } });
                        }

                        driverPin.Position = new Position(Convert.ToDouble(result.lat), Convert.ToDouble(result.lng));
                        EstimatedTime.Text = !string.IsNullOrEmpty(result.distance) ? (Convert.ToDouble(result.distance) < 1000) ? $"{result.distance} m" : $"{Convert.ToDouble(result.distance) / 1000} km" : "-";
                        EstimatedBlocks.Text = !string.IsNullOrEmpty(result.waiting_time) ? (Convert.ToDouble(result.waiting_time) < 1) ? $"Llegando" : $"{result.waiting_time} min" : "-";

                        if (Convert.ToDouble(result.distance) < 100 && !nearCarShown)
                        {
                            nearCarShown = true;
                            await _navigationService.NavigateAsync("Trip_InformationPopUp", new NavigationParameters { { "Msg", "¡Su movil esta en la puerta!" } });
                        }
                        map.Polylines.Clear();
                        await DrawRoute(driverLoc, userLoc, "999");
                        AdjustCamera(orgLoc, dstLoc);
                    });
                    break;

                case "2":
                    Device.BeginInvokeOnMainThread( async() =>
                    {
                        if (state != 2)
                        {
                            state = 2;
                            btnBoard.IsVisible = true;
                            await _navigationService.NavigateAsync("Trip_InformationPopUp", new NavigationParameters { { "Msg", "¡Su movil se encuentra esperandolo!" } });
                        }
                    });
                    break;

                case "4":
                    await _navigationService.NavigateAsync("/SideMenuPage", animated: true);
                    break;
            }
            timer?.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
        }

        public void TripAccepted(int state)
        {
            if (state == 1)
                Device.BeginInvokeOnMainThread(() => lblState.Text = "¡TU REMIS ESTÁ EN CAMINO!");
        }         

        private void CallClicked(object sender, EventArgs e)
        {
            
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            
        }
        private void BoardClicked(object sender, EventArgs e)
        {

        }
        protected override bool OnBackButtonPressed() => true;

        private void AdjustCamera(Location org, Location dst)
        {
            try
            {

                // Define la distancia mínima a mostrar en el mapa
                const double distanciaMinimaEnKm = 0.2;

                // Calcula la distancia real entre los puntos
                var distanciaEnKm = Location.CalculateDistance(org.Latitude, org.Longitude, dst.Latitude, dst.Longitude, DistanceUnits.Kilometers);

                // Calcula el nivel de zoom necesario para mostrar la distancia mínima
                var radioEnKm = Math.Max(distanciaEnKm / 2, distanciaMinimaEnKm);
                var zoom = Math.Log(40075 / (256 * radioEnKm)) / Math.Log(2);

                // Calcula la posición del centro del mapa
                var latitudMedia = (org.Latitude + dst.Latitude) / 2 - 0.001; // Desplaza hacia arriba en 0.01 grados
                var longitudMedia = (org.Longitude + dst.Longitude) / 2;

                // Mueve el mapa al centro y establece el zoom adecuado
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitudMedia, longitudMedia), Distance.FromKilometers(radioEnKm + 0.2)));
            }
            catch (Exception) { }
        }

        public async Task DrawRoute(Location org, Location dst, string hexColor)
        {
            try
            {
                var googleDirection = await Places.GetDirections(org.Latitude.ToString(), org.Longitude.ToString(), dst.Latitude.ToString(), dst.Longitude.ToString());
                if (googleDirection.Routes != null && googleDirection.Routes.Count > 0)
                {
                    var positions = (Enumerable.ToList(PolylineHelper.Decode(googleDirection.Routes.First().OverviewPolyline.Points)));

                    var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
                    polyline.StrokeColor = Color.FromHex(hexColor);
                    polyline.StrokeWidth = 3;

                    foreach (var p in positions)
                        polyline.Positions.Add(p);

                    map.Polylines.Add(polyline);
                }
            }
            catch (Exception) { }
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}