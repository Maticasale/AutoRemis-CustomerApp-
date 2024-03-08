using AutoRemis.Helpers;
using AutoRemis.Models;
using AutoRemis.Models.Google;
using AutoRemis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace AutoRemis.Views
{
    public partial class TestPage2 : ContentPage
    {
        double lat_destination, lng_destination;
        private User user;
        public TestPage2()
        {
            InitializeComponent();
            user = AppStateManager.GetUser();

            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(latitude: -31.220112, longitude: -61.512372), 14d);

            lat_destination = -31.224848;
            lng_destination = -61.485089;
            TrackPath();
        }

        async void TrackPath()
        {
            var pathcontent = await LoadRoute();
            map.Polylines.Clear();

            try
            {
                var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
                polyline.StrokeColor = Color.FromHex("B1D506");
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
                    lat_destination,
                    lng_destination,
                    DistanceUnits.Kilometers);

                // Calcula el nivel de zoom necesario para mostrar la distancia mínima
                var radioEnKm = Math.Max(distanciaEnKm / 2, distanciaMinimaEnKm);
                var zoom = Math.Log(40075 / (256 * radioEnKm)) / Math.Log(2);

                // Calcula la posición del centro del mapa
                var latitudMedia = (user.lastKnownPosition.Latitude + lat_destination) / 2; // Desplaza hacia arriba en 0.01 grados
                var longitudMedia = (user.lastKnownPosition.Longitude + lng_destination) / 2;

                // Mueve el mapa al centro y establece el zoom adecuado
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitudMedia, longitudMedia), Distance.FromKilometers(radioEnKm + 0.5)));
            }
            catch (Exception) { }
        }

        internal async Task<List<Position>> LoadRoute()
        {
            try
            {
                var googleDirection = await Places.GetDirections(user.lastKnownPosition.Latitude.ToString(), user.lastKnownPosition.Longitude.ToString(), Convert.ToString(lat_destination), Convert.ToString(lng_destination));
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
    }
}
