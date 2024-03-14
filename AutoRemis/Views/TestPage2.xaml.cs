using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Services;
using Prism.Navigation;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace AutoRemis.Views
{
    public partial class TestPage2 : ContentPage
    {
        double lat_destination, lng_destination;
        double lat_car, lng_car;
        private User user;
        Location UserLoc, Destination, DriverLoc;

        private readonly INavigationService _navigationService;

        public TestPage2(INavigationService navigationService)
        {
            InitializeComponent();
            user = AppStateManager.GetUser();
            _navigationService = navigationService;


            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(latitude: -31.220112, longitude: -61.512372), 14d);
            map.MyLocationEnabled = false;
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.MyLocationButtonEnabled = false;

            stepDoor.Status = Syncfusion.XForms.ProgressBar.StepStatus.InProgress;
            stepDoor.ProgressValue = 50;


            UserLoc = new Location()
            {
                Latitude = user.lastKnownPosition.Latitude,
                Longitude = user.lastKnownPosition.Longitude
            };
            Destination = new Location()
            {
                Latitude = -31.224848,
                Longitude = -61.485089
            };
            DriverLoc = new Location()
            {
                Latitude = -31.257245,
                Longitude = -61.501679
            };

            map.Pins.Add(new Pin()
            {
                Type = PinType.SearchResult,
                Position = new Position(Destination.Latitude, Destination.Longitude),
                Label = "Destino",
                Icon = BitmapDescriptorFactory.FromBundle("pinLoc.png")
            });

            map.Pins.Add(new Pin()
            {
                Type = PinType.SearchResult,
                Position = user.lastKnownPosition,
                Label = "Origen",
                Icon = BitmapDescriptorFactory.FromBundle("pinDot.png"),                
            });

            AdjustCamera(UserLoc, Destination, "000");
            AddMapStyle();

            MessagingCenter.Subscribe<object, int>(this, "TripAccepted", (sender, args) => TripAccepted(args));
        }

        private void TripAccepted(int args)
        {
            if (args == 1)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    stepDoor.ProgressValue = 100;
                    await Task.Delay(100);
                    await AdjustCamera(DriverLoc, UserLoc, "999");
                    stepView3.Status = Syncfusion.XForms.ProgressBar.StepStatus.InProgress;
                    stepView3.ProgressValue = 50;
                    await Task.WhenAll(DriverInfoBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                    lblMsg.Text = "Su Chofer se encuentra en camino, le mantendremos informado";
                });
                map.Pins.Add(new Pin()
                {
                    Type = PinType.SearchResult,
                    Position = new Position(DriverLoc.Latitude, DriverLoc.Longitude),
                    Label = "Origen",
                    Icon = BitmapDescriptorFactory.FromBundle("pinCar.png"),
                    Rotation = 100
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    stepView3.ProgressValue = 100;
                    await Task.Delay(100);
                    stepView3.Status = Syncfusion.XForms.ProgressBar.StepStatus.Completed;
                    lblMsg.Text = "Su chofer se encuentra en puerta esperandolo";
                    await AdjustCamera(DriverLoc, UserLoc, "999");

                    Grid.SetColumnSpan(btnCancel, 1);
                    Grid.SetColumn(btnCancel, 0);
                    btnBoard.IsVisible = true;
                    Grid.SetColumn(btnBoard, 1);
                });
            }
        }

        private async void Action(object sender, EventArgs e)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(user.lastKnownPosition, Distance.FromMiles(0.3)));
            await Task.Delay(1000);
            await _navigationService.NavigateAsync("Trip_StateInfoPopUp", new NavigationParameters { { "Msg", "Su movil se encuentra en puerta esperandolo" }, { "Type", 0 } });
        }

        private async void CancelClicked(object sender, EventArgs e)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(user.lastKnownPosition, Distance.FromMiles(0.3)));
            await Task.Delay(1000);
            await _navigationService.NavigateAsync("Trip_StateInfoPopUp", new NavigationParameters { { "Msg", "Su pedido ha sido recepcionado exitosamente" }, { "Type", 1 } });
        }

        public void AddMapStyle()
        {
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"AutoRemis.Common.MapResources.MapTheme.json");
            string styleFile;
            using (var reader = new System.IO.StreamReader(stream))
            {
                styleFile = reader.ReadToEnd();
            }

            map.MapStyle = MapStyle.FromJson(styleFile);
        }

        async Task AdjustCamera(Location org, Location dst, string hexColor)
        {
            await DrawRoute(org, dst, hexColor);

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
                var latitudMedia = (org.Latitude + dst.Latitude) / 2 - 0.002; // Desplaza hacia arriba en 0.01 grados
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
    }
}