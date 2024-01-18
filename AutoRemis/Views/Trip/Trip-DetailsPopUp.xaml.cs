using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using Prism.Navigation;
using Rg.Plugins.Popup.Extensions;
using AutoRemis.Models.Google;
using AutoRemis.Models;
using DryIoc;
using System.Numerics;

namespace AutoRemis.Views
{
    public partial class Trip_DetailsPopUp : PopupPage, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private Trip trip;

        public Trip_DetailsPopUp(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            trip = parameters.GetValue<Trip>("Trip");
        }


        private async void NextClicked(object sender, System.EventArgs e)
        {
            trip.block = entryBlock.Text;
            trip.tower = entryTorre.Text;
            trip.flor = entryPiso.Text;
            trip.flats_block = entryDepto.Text;
            trip.flat = entryManzana.Text;
            trip.lot = entryLote.Text;
            trip.complex = entryComplejo.Text;
            trip.house = entryCasaDuplex.Text;
            trip.country = entryCountryBarrio.Text;

            await Navigation.PopPopupAsync();
            await _navigationService.NavigateAsync("Trip_ConfigPage", new NavigationParameters { { "Trip", trip } });
        }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}
