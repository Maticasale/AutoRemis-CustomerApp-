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
            user = AppStateManager.GetUser();

        }

        private void CancelClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(user.lastKnownPosition, Distance.FromKilometers(14d)));

                lblState.Text = "¡PEDIDO RECEPCIONADO!";
                //bx1.IsVisible = false;
                //bx2.IsVisible = false;
                //gif.IsVisible = false;
                //ContainmentBox.IsVisible = false;

                DriverInfoBox.IsVisible = true;
            });
        }

        private void CallClicked(object sender, EventArgs e)
        {

        }
    }
}
