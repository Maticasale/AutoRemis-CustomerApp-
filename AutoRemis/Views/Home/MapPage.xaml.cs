using AutoRemis.Models.Google;
using AutoRemis.Models;
using AutoRemis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Prism.Navigation;
using Prism.Ioc;
using AutoRemis.Helpers;
using Rg.Plugins.Popup.Extensions;
using AutoRemis.Models.Services;

namespace AutoRemis.Views
{
    public partial class MapPage : ContentPage
    {
        private readonly INavigationService _navigationService;

        private User user;
        private AppSettings app;
        Location UserPosition;

        private List<Image> checkboxImages; 
        private List<Frame> checkboxFrames;

        private string LatOrg, LngOrg, AdressOrg, NumberOrg;

        private string LatDst, LngDst, AdressDst, NumberDst;

        string EntryFocused;
        bool ItemSellected;
        public MapPage()
        {
            InitializeComponent();
            LoadUI();
            _navigationService = Prism.PrismApplicationBase.Current.Container.Resolve<INavigationService>();
        }

        protected override bool OnBackButtonPressed() => true;

        private async void LoadUI()
        {
            //Variables 
            checkboxImages = new List<Image> { imgStandard, imgCapacDif, imgEcologic, imgExecutive };
            checkboxFrames = new List<Frame> { frmStandard, frmCapacDif, frmEcologic, frmExecutive };

            //User and App Data
            user = AppStateManager.GetUser();
            app = AppStateManager.GetAppInfo();

            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(user.lastKnownPosition, 14d);

            UserPosition = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High));

            map.MoveToRegion(MapSpan.FromCenterAndRadius(user.lastKnownPosition, Distance.FromMiles(0.3)));

            //General UI Settings
            map.MyLocationEnabled = true;
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.MyLocationButtonEnabled = true;
            LoadTaxis();

            //SearchBars Settings
            EntryFocused = "Org";

            EntryOrg.ApiKey = app.GlobalApiKey;
            EntryOrg.Type = PlaceType.All;
            EntryOrg.Components = new Components("country:ar");
            EntryOrg.PlacesRetrieved += Search_Bar_PlacesRetrieved;
            EntryOrg.TextChanged += Search_Bar_TextChanged;
            EntryOrg.MinimumSearchText = 3;
            results_list.ItemSelected += Results_List_ItemSelected;

            EntryDst.ApiKey = app.GlobalApiKey;
            EntryDst.Type = PlaceType.All;
            EntryDst.Components = new Components("country:ar");
            EntryDst.PlacesRetrieved += Search_Bar_PlacesRetrieved;
            EntryDst.TextChanged += Search_Bar_TextChanged;
            EntryDst.MinimumSearchText = 3;
            results_list.ItemSelected += Results_List_ItemSelected;

            //Map Events
            map.CameraMoveStarted += async (sender, e) => await CloseResultBox();
            map.PinClicked += async (sender, e) => await CloseResultBox();
            map.MapClicked += async (sender, e) => await CloseResultBox();
            map.CameraIdled += async (sender, e) =>
            {
                ItemSellected = false;
                await CloseResultBox();
                var p = e.Position;
                try
                {
                    var response = await Places.GetPlaceID(p.Target.Latitude.ToString(), p.Target.Longitude.ToString());
                    FillEntrys(response, new Position(latitude: p.Target.Latitude, longitude: p.Target.Longitude));
                }
                catch (Exception) { }
            };
        }

        private async void LoadTaxis()
        {
            var response = await CarService.GetNearCars(new NearCar() { lat_device = user.lastKnownPosition.Latitude.ToString(), lng_device = user.lastKnownPosition.Longitude.ToString(), lat_origin = user.lastKnownPosition.Latitude.ToString(), lng_origin = user.lastKnownPosition.Longitude.ToString() });

            if (response.ServiceState == ServiceType.CheckOut)
            {
                foreach (var item in response.NearCars)
                {
                    Pin VehiclePins = new Pin()
                    {
                        Label = "ID: " + item.id_movil,
                        Type = PinType.Place,
                        Icon = BitmapDescriptorFactory.FromBundle("pinDriver.png"),
                        Position = new Position(Convert.ToDouble(item.lat), Convert.ToDouble(item.lng)),
                    };
                    map.Pins.Add(VehiclePins);
                }
            }
            else
                RiseErrorMsg("¡Advertencia!", "Parece que no hay autos por aqui", 3, SoundHelper.SoundType.Alert);
        }

        private async void NextClicked(object sender, EventArgs e)
        {
            if (AdressOrg != null && LatOrg != null && LngOrg != null)
            {
                Trip trip = new Trip();
                if (!string.IsNullOrWhiteSpace(EntryDst.Text))
                {
                    if (AdressDst != null || LatDst != null || LngDst != null)
                    {
                        trip.lat_destination = LatDst;
                        trip.lng_destination = LngDst;
                        trip.address_destination = EntryDst.Text;
                        trip.address_number_destination = NumberDst;
                    }
                    else
                    {
                        RiseErrorMsg("¡Aviso!", "Parece que quisiste escribir un destino pero no hemos podido encontrar dicha información, por favor asegurate de que este bien escrito y que hayas seleccionado alguno de la lista", 6, SoundHelper.SoundType.Alert);
                        return;
                    }
                }

                trip.address_origin = EntryOrg.Text;
                trip.address_number_origin = NumberOrg;
                trip.lat_origin = LatOrg;
                trip.lng_origin = LngOrg;
                trip.name = user.FullName;
                trip.user = user.FullName;
                trip.phone = user.PhoneNumber;

                await _navigationService.NavigateAsync("Trip_DetailsPopUp", new NavigationParameters { { "Trip", trip } });
            }
            else
                RiseErrorMsg("¡Aviso!", "Necesitamos que completes al menos el origen para poder pedir un viaje", 4, SoundHelper.SoundType.Alert);
        }

        private void OnCheckboxTapped(object sender, EventArgs e)
        {
            var frameIndex = Array.IndexOf(new Frame[] { frmStandard, frmCapacDif, frmEcologic, frmExecutive }, (Frame)sender);

            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var checkboxImage in checkboxImages)
                    checkboxImage.Source = "iconEmptyChk.png";

                foreach (var checkboxFrame in checkboxFrames)
                {
                    checkboxFrame.BackgroundColor = Color.White;
                    checkboxFrame.IsEnabled = true;
                }

                checkboxFrames[frameIndex].BackgroundColor = Color.FromHex("#eaebea");
                checkboxImages[frameIndex].Source = "iconFilledChk.png";
                checkboxFrames[frameIndex].IsEnabled = false;
            });
        }

        private async void Search_Bar_PlacesRetrieved(object sender, AutoCompleteResult result)
        {
            if (ItemSellected)
                result.AutoCompletePlaces = null;

            results_list.ItemsSource = result.AutoCompletePlaces;

            if (result.AutoCompletePlaces != null && result.AutoCompletePlaces.Count > 0)
                await OpenResultBox(result.AutoCompletePlaces.Count);
            else
                await CloseResultBox();

            ItemSellected = false;
        }

        private async void Search_Bar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntryOrg.Text) || string.IsNullOrWhiteSpace(EntryDst.Text) || !string.IsNullOrEmpty(e.NewTextValue))
                await CloseResultBox();
        }

        private async void Results_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            var prediction = (AutoCompletePrediction)e.SelectedItem;
            results_list.SelectedItem = null;

            FillEntrys(prediction.Place_ID, new Position());

            await CloseResultBox();
        }

        private async void FillEntrys(string PlaceID, Position alternativePosition)
        {
            Place place = await Places.GetPlace(PlaceID);

            if (place != null && !ItemSellected)
            {
                if (EntryFocused == "Org")
                {
                    try
                    {
                        LatOrg = place.Latitude == null ? alternativePosition.Latitude.ToString() : place.Latitude.ToString();
                        LngOrg = place.Longitude == null ? alternativePosition.Longitude.ToString() : place.Longitude.ToString();
                        AdressOrg = place.StreetName == null ? place.FormattedAddress : place.StreetName.ToString();
                        NumberOrg = (place.StreetNumber == null || place.StreetName == null) ? "" : place.StreetNumber.ToString();
                        EntryOrg.Text = (place.StreetNumber == null || place.StreetName == null) ? place.FormattedAddress : $"{place.StreetName} {place.StreetNumber}, {place.Locality.ShortName}";
                    }
                    catch (Exception) { }
                }
                else
                {
                    try
                    {
                        LatDst = place.Latitude == null ? alternativePosition.Latitude.ToString() : place.Latitude.ToString();
                        LngDst = place.Longitude == null ? alternativePosition.Longitude.ToString() : place.Longitude.ToString();
                        AdressDst = place.StreetName == null ? place.FormattedAddress : place.StreetName.ToString();
                        NumberDst = (place.StreetNumber == null || place.StreetName == null) ? "" : place.StreetNumber.ToString();
                        EntryDst.Text = (place.StreetNumber == null || place.StreetName == null) ? place.FormattedAddress : $"{place.StreetName} {place.StreetNumber}, {place.Locality.ShortName}";
                    }
                    catch (Exception) { }
                }
                ItemSellected = true;
            }
            else
                return;
        }

        private async Task OpenResultBox(int Results)
        {
            if (!EntryDst.IsFocused && !EntryOrg.IsFocused) return;

            await Task.WhenAll(ResultBox.FadeTo(1));
            Search.CornerRadius = new CornerRadius(5, 5, 0, 0);
            switch (Results)
            {
                case 0:
                    ResizeStackLayout(0);
                    break;
                case 1:
                    ResizeStackLayout(45);
                    break;
                case 2:
                    ResizeStackLayout(90);
                    break;
                case 3:
                    ResizeStackLayout(135);
                    break;
                case 4:
                    ResizeStackLayout(180);
                    break;
                default:
                    ResizeStackLayout(180);
                    break;
            }
        }

        private async Task CloseResultBox()
        {
            await Task.WhenAll(ResultBox.FadeTo(0));
            Search.CornerRadius = 5;
            ResizeStackLayout(0);
        }

        private void ResizeStackLayout(double heightRequest) => MainThread.BeginInvokeOnMainThread(() => ResultBox.HeightRequest = heightRequest);

        void EntryOrgFocused(Object sender, FocusEventArgs e) => EntryFocused = "Org";

        void EntryDstFocused(Object sender, FocusEventArgs e) => EntryFocused = "Dst";
        private void RiseErrorMsg(string title, string msg, int time, SoundHelper.SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                stkOptions.IsVisible = false;

                Title.Text = title;
                Msg.Text = msg;

                switch (type)
                {
                    case SoundHelper.SoundType.Error:
                        imgItem.Source = "iconError.png";
                        CancellBox.BorderColor = Color.FromHex("#ff355b");
                        CancellBox.BackgroundColor = Color.FromHex("#fffbfc");
                        Title.TextColor = Color.FromHex("#ff355b");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Alert:
                        imgItem.Source = "iconWarning.png";
                        CancellBox.BorderColor = Color.FromHex("#FFC021");
                        CancellBox.BackgroundColor = Color.FromHex("#fffefb");
                        Title.TextColor = Color.FromHex("#FFC021");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Success:
                        imgItem.Source = "iconSuccess.png";
                        CancellBox.BorderColor = Color.FromHex("#47D764");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfefc");
                        Title.TextColor = Color.FromHex("#47D764");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Success);
                        break;
                    case SoundHelper.SoundType.Message:
                        imgItem.Source = "iconInfo.png";
                        CancellBox.BorderColor = Color.FromHex("#2F86EB");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfdff");
                        Title.TextColor = Color.FromHex("#2F86EB");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Message);
                        break;
                }

                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

                SoundHelper.StopCurrentSound();

                stkOptions.IsVisible = true;
            });
        }

    }
}
