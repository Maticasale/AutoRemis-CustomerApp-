using AutoRemis.Helpers;
using AutoRemis.Models;
using Prism.Navigation;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace AutoRemis.Views
{
    public partial class Trip_RatePage : ContentPage, INavigatedAware
    {
        private User user;

        private bool[] observations;

        public Trip_RatePage()
        {
            InitializeComponent();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }


        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //User Data
            user = AppStateManager.GetUser();

            //Variables
            observations = new bool[10];
        }

        private void ObservationTapped(object sender, EventArgs e)
        {
            var obs = (PancakeView)sender;

            Device.BeginInvokeOnMainThread(() =>
            {
                int index = int.Parse(obs.ClassId);

                observations[index] = !observations[index];

                obs.BackgroundColor = observations[index] ? Color.FromHex("#e8f0cb") : Color.White;
                string result = string.Join("", observations.Select(b => b ? "1" : "0"));
            });
        }

        private void FinishTrip(object sender, EventArgs e)
        {

        }

        private void RateTrip(object sender, EventArgs e)
        {

        }
    }
}
