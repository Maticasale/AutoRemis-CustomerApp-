using AutoRemis.Models.Google;
using System;
using Xamarin.Forms;
using AutoRemis.Models;
using Xamarin.Forms.GoogleMaps;
using System.Collections.Generic;
using ImTools;
using AutoRemis.Models.Services;
using Xamarin.Essentials;
using AutoRemis.Services;
using System.IO;
using System.Net;
using Prism.Navigation;

namespace AutoRemis.Views
{
    public partial class TestPage : ContentPage
    {
        User user;
        Position NW, NE, SW, SE;
        List<Position> Posiciones = new List<Position>() { };
        List<string> nombresPuntosCardinales = new List<string> { "NW", "NE", "SW", "SE" };
        private readonly INavigationService _navigationService;

        public TestPage(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;

            NW = new Position(latitude: -31.219990, longitude: -61.512660); //-31.219990, -61.512660
            NE = new Position(latitude: -31.231685, longitude: -61.444430); //-31.231685, -61.444430
            SW = new Position(latitude: -31.286355, longitude: -61.527472); //-31.286355, -61.527472
            SE = new Position(latitude: -31.296703, longitude: -61.459668); //-31.296703, -61.459668

            Posiciones.Add(NW);
            Posiciones.Add(NE);
            Posiciones.Add(SW);
            Posiciones.Add(SE);


            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(latitude: -31.220112, longitude: -61.512372), 14d);

            map.CameraIdled += (sender, e) =>
            {
                var p = e.Position;
                if (EstaDentroDelRectangulo(p.Target))
                    OKLBL.Text = "SI";
                else 
                    OKLBL.Text = "NO";
            };
            DrawBox();
            DrawPins();

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await _navigationService.NavigateAsync("AviableUpdatePopUp", new NavigationParameters { { "hardUpdate", false }, { "newVersion", "1.1.2" } });

            ////LoginUser user = new LoginUser() { id = "1234", appVersion = VersionTracking.CurrentVersion, tipo = "USUARIO", token = "fN1TF_WNQ6ukgFpkz2gU9G:APA91bG6tUNdqoEAXl44khcUdpZ9-NFNpYmY9snDu_H5V2Q3UAjkYsvy--UxusxycG5jWGkD_tCo9mt1KM7KBFNdUfK3gu0CxzAAXlaMj1g0iIA5uMd2dEKPr5v2uXk2WsZJvPr_kiHW" };

            ////var response = await Auth.Login(user);

            //double latitude = 40.712776; // Latitud del punto
            //double longitude = -74.005974; // Longitud del punto
            //string apiKey = "AIzaSyAA4fS33bAgNtHXOOA_19ODwmHK3W0cfGQ"; // Reemplaza con tu clave de API

            //string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={apiKey}";

            //WebRequest request = WebRequest.Create(url);
            //WebResponse response = request.GetResponse();
            //Stream data = response.GetResponseStream();
            //StreamReader reader = new StreamReader(data);

            //// Leer la respuesta JSON desde la API de Google Maps
            //string responseFromServer = reader.ReadToEnd();
            //response.Close();

            //// Analizar la respuesta JSON y extraer la dirección o el bearing
            //// (debes implementar esta parte según tus necesidades)

            //Console.WriteLine("Respuesta de la API de Geocodificación Inversa:");
            //Console.WriteLine(responseFromServer);

        }

        // Método para verificar si una nueva posición está dentro del área definida por un rectángulo
        private bool EstaDentroDelRectangulo(Position NP)
        {
            // Calcula las coordenadas del bounding box
            double bb_ix = Math.Min(Math.Min(NW.Latitude, NE.Latitude), Math.Min(SW.Latitude, SE.Latitude));
            double bb_ax = Math.Max(Math.Max(NW.Latitude, NE.Latitude), Math.Max(SW.Latitude, SE.Latitude));
            double bb_iy = Math.Min(Math.Min(NW.Longitude, NE.Longitude), Math.Min(SW.Longitude, SE.Longitude));
            double bb_ay = Math.Max(Math.Max(NW.Longitude, NE.Longitude), Math.Max(SW.Longitude, SE.Longitude));

            // Verifica si el punto está dentro del bounding box
            return bb_ix <= NP.Latitude && NP.Latitude <= bb_ax &&
                   bb_iy <= NP.Longitude && NP.Longitude <= bb_ay;
        }

        public void DrawBox()
        {
            var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
            polyline.StrokeColor = Color.Black;
            polyline.StrokeWidth = 5;

            polyline.Positions.Add(NW);
            polyline.Positions.Add(NE);
            polyline.Positions.Add(SE);
            polyline.Positions.Add(SW);
            polyline.Positions.Add(NW); 
            map.Polylines.Add(polyline);
        }

        public void DrawPins()
        {
            foreach (var pos in Posiciones) 
            {
                map.Pins.Add(new Pin()
                {
                    Type = PinType.SearchResult,
                    Position = pos,
                    Label = nombresPuntosCardinales[Posiciones.IndexOf(pos)],
                    Icon = BitmapDescriptorFactory.FromBundle("pinDriver.png")
                });
            }
        }                
    }
}
