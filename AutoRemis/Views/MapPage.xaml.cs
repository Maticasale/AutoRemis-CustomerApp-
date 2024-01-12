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

namespace AutoRemis.Views
{
    public partial class MapPage : ContentPage
    {
        private List<Image> checkboxImages; 
        private List<Frame> checkboxFrames;
        private readonly INavigationService _navigationService;
        private User user;



        string LatOrg;
        string LngOrg;
        string AdressOrg;
        string NumberOrg;

        string LatDst;
        string LngDst;
        string AdressDst;
        string NumberDst;

        bool SearchBarsFoucsed = false;
        bool AutoFillEnable = false;

        string EntryFocused;
        bool ItemSellected;
        public MapPage()
        {
            InitializeComponent();
            LoadUI();
            _navigationService = Prism.PrismApplicationBase.Current.Container.Resolve<INavigationService>();
        }

        private void LoadUI()
        {
            //Variables
            checkboxImages = new List<Image> { imgStandard, imgCapacDif, imgEcologic, imgExecutive };
            checkboxFrames = new List<Frame> { frmStandard, frmCapacDif, frmEcologic, frmExecutive };

            //User Data
            user = AppStateManager.GetUser();

            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(user.lastKnownPosition, 14d);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(user.lastKnownPosition, Distance.FromMiles(0.3)));

            //General UI Settings
            map.MyLocationEnabled = true;
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.MyLocationButtonEnabled = true;

            //SearchBars Settings
            EntryFocused = "Org";

            EntryOrg.ApiKey = "AIzaSyDxKLNaQ8S-k2D7MY0dvxMbRYWtuRQV0PI";
            EntryOrg.Type = PlaceType.All;
            EntryOrg.Components = new Components("country:ar");
            EntryOrg.PlacesRetrieved += Search_Bar_PlacesRetrieved;
            EntryOrg.TextChanged += Search_Bar_TextChanged;
            EntryOrg.MinimumSearchText = 2;
            results_list.ItemSelected += Results_List_ItemSelected;

            EntryDst.ApiKey = "AIzaSyDxKLNaQ8S-k2D7MY0dvxMbRYWtuRQV0PI";
            EntryDst.Type = PlaceType.All;
            EntryDst.Components = new Components("country:ar");
            EntryDst.PlacesRetrieved += Search_Bar_PlacesRetrieved;
            EntryDst.TextChanged += Search_Bar_TextChanged;
            EntryDst.MinimumSearchText = 2;
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
                    var response = await Places.GetPlaceID(p.Target.Latitude.ToString(), p.Target.Longitude.ToString(), "AIzaSyDxKLNaQ8S-k2D7MY0dvxMbRYWtuRQV0PI");
                    if (AutoFillEnable)
                        FillEntrys(response, new Position(latitude: p.Target.Latitude, longitude: p.Target.Longitude));
                }
                catch (Exception) { }
            };
        }

        private void OnCheckboxTapped(object sender, EventArgs e)
        {
            var frameIndex = Array.IndexOf(new Frame[] { frmStandard, frmCapacDif, frmEcologic, frmExecutive }, (Frame)sender);

            if (frameIndex >= 0 && frameIndex < checkboxImages.Count)
            {
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
        }

        private async void Search_Bar_PlacesRetrieved(object sender, AutoCompleteResult result)
        {
            if (ItemSellected)
                result.AutoCompletePlaces = null;

            results_list.ItemsSource = result.AutoCompletePlaces;
            spinner.IsRunning = false;
            spinner.IsVisible = false;

            if (result.AutoCompletePlaces != null && result.AutoCompletePlaces.Count > 0 && SearchBarsFoucsed)
                await OpenResultBox(result.AutoCompletePlaces.Count);
            else
                await CloseResultBox();

            ItemSellected = false;
        }

        private async void Search_Bar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue) && !SearchBarsFoucsed)
            {
                await CloseResultBox();
                spinner.IsVisible = true;
                spinner.IsRunning = true;
            }
            else
            {
                spinner.IsRunning = false;
                spinner.IsVisible = false;
            }

            if ((EntryFocused == "Org" && string.IsNullOrWhiteSpace(EntryOrg.Text)) || (EntryFocused == "Dst" && string.IsNullOrWhiteSpace(EntryDst.Text)))
                await CloseResultBox();
        }

        private async void Results_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var prediction = (AutoCompletePrediction)e.SelectedItem;
            results_list.SelectedItem = null;

            FillEntrys(prediction.Place_ID, new Position());

            await CloseResultBox();
        }

        private async void FillEntrys(string PlaceID, Position alternativePosition)
        {
            //if (!AutoFillEnable)
            //    return;

            Place place = await Places.GetPlace(PlaceID, "AIzaSyDxKLNaQ8S-k2D7MY0dvxMbRYWtuRQV0PI");

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
            await Task.WhenAll(ResultBox.FadeTo(1, length: 100));
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
            await Task.WhenAll(ResultBox.FadeTo(0, length: 100));
            Search.CornerRadius = 5;
            ResizeStackLayout(0);
        }

        private void ResizeStackLayout(double heightRequest) => MainThread.BeginInvokeOnMainThread(() => ResultBox.HeightRequest = heightRequest);

        void EntryOrgFocused(Object sender, FocusEventArgs e)
        {
            EntryFocused = "Org";
            SearchBarsFoucsed = true;
        }

        void EntryDstFocused(Object sender, FocusEventArgs e)
        {
            EntryFocused = "Dst";
            SearchBarsFoucsed = true;
        }
    }
}
