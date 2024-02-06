using AutoRemis.Models.Google;
using AutoRemis.Services;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using AutoRemis.Models;
using Prism.Navigation;
using AutoRemis.Helpers;

namespace AutoRemis.Views
{
    public partial class TestPage : ContentPage
    {
        User user;
        public TestPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, FirebaseMessage>(this, "FCM", OnTripParamsChanged);
        }

        private void OnTripParamsChanged(object arg, FirebaseMessage trip)
        {

        }
    }
}
