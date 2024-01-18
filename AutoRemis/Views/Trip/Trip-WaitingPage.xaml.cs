using AutoRemis.Helpers;
using AutoRemis.Models;
using FFImageLoading.Svg.Forms;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.PlatformConfiguration;

namespace AutoRemis.Views
{
    public partial class Trip_WaitingPage : ContentPage, INavigatedAware
    {
        private User user;
        private Trip trip;

        public Trip_WaitingPage()
        {
            InitializeComponent();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Variables
            trip = parameters.GetValue<Trip>("Trip");

            //User Data
            user = AppStateManager.GetUser();

            map.MoveToRegion(MapSpan.FromCenterAndRadius(user.lastKnownPosition, Distance.FromMiles(0.3)));
            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(user.lastKnownPosition, 14d);

            //General UI Settings
            map.IsEnabled = true;
            map.UiSettings.ZoomControlsEnabled = false;

            Device.BeginInvokeOnMainThread(() => stk.Children.Add(new SvgCachedImage { Aspect = Aspect.AspectFit, Source = "gifRadar.gif", VerticalOptions = LayoutOptions.Center }));
        }
        public void OnNavigatedFrom(INavigationParameters parameters) {}

    }
}
