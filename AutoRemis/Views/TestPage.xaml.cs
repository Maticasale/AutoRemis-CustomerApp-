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
            //await _navigationService.NavigateAsync("AviableUpdatePopUp", new NavigationParameters { { "hardUpdate", false }, { "newVersion", "1.1.2" } });

            var result = await AuthService.Register(new RegisterUser() { appVersion = VersionTracking.CurrentVersion, email = "matiascasale1@gmail.com", fullName = "Matias Casale", phoneNumber = "+5493564568057", usrFcb = "", token = "cFTwKqD4SjakVJVRWCWSu6:APA91bExWBQzu_6kUjP8lASC804tbAZSN0Be5EucXZAnOCGwP_8cL3GW7c3Pal3RrsPSGTm-WNGUcW2k4bw0eU93BMBOG_60zn51W82yeC9fQHxPSx81eUFmRRjxUhLx1pZBdVzLP-Xc" });

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
