using AutoRemis.Models.Google;
using AutoRemis.Services;
using Prism.Navigation;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using AutoRemis.Models;
using Rg.Plugins.Popup.Extensions;
using System.Collections.Generic;

namespace AutoRemis.Views
{
    public partial class Trip_ChangeMainParamsPopUp : PopupPage, INavigationAware
    {
        private readonly INavigationService _navigationService;

        private Trip trip;

        private string LatOrg, LngOrg, AdressOrg, NumberOrg;

        private string LatDst, LngDst, AdressDst, NumberDst;

        string EntryFocused;
        bool ItemSellected;

        public Trip_ChangeMainParamsPopUp(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Variables
            trip = parameters.GetValue<Trip>("Trip");

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

            //General UI Settings
            EntryOrg.Text = trip.address_origin;
            EntryDst.Text = trip.address_destination;
        }
        private async void CancelClicked(object sender, EventArgs e) => await Navigation.PopPopupAsync();
        private async void OkClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<object, Trip>(this, "Trip", trip);
            await Navigation.PopPopupAsync();
        }

        #region SearchBarFunctions
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

                        trip.address_number_origin = NumberOrg;
                        trip.address_origin = EntryOrg.Text;
                        trip.lat_origin = LatOrg;
                        trip.lng_origin = LngOrg;
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

                        trip.address_number_destination = NumberDst;
                        trip.address_destination = EntryDst.Text;
                        trip.lat_destination = LatDst;
                        trip.lng_destination = LngDst;
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

            btnCancel.IsVisible = false;
            btnOK.IsVisible = false;

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
                    ResizeStackLayout(85);
                    break;
                case 3:
                    ResizeStackLayout(145);
                    break;
                case 4:
                    ResizeStackLayout(190);
                    break;
                default:
                    ResizeStackLayout(210);
                    break;
            }
        }

        private async Task CloseResultBox()
        {
            await Task.WhenAll(ResultBox.FadeTo(0));
            Search.CornerRadius = 5;
            ResizeStackLayout(0);

            btnOK.IsVisible = true;
            btnCancel.IsVisible = true;
        }
        private void ResizeStackLayout(double heightRequest) => MainThread.BeginInvokeOnMainThread(() => ResultBox.HeightRequest = heightRequest);

        void EntryOrgFocused(Object sender, FocusEventArgs e) => EntryFocused = "Org";

        void EntryDstFocused(Object sender, FocusEventArgs e) => EntryFocused = "Dst";

        #endregion

        public void OnNavigatedFrom(INavigationParameters parameters) {}
    }
}
