using AutoRemis.Models;
using Prism.Navigation;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms;

namespace AutoRemis.Views
{
    public partial class Trip_StateInfoPopUp : PopupPage, INavigationAware
    {
        private readonly INavigationService _navigationService;

        private string msg;
        private int msgType;
        public Trip_StateInfoPopUp(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            BackgroundClicked += OnBackgroundClicked;
        }

        private void OnBackgroundClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<object, int>(this, "TripAccepted", msgType);

            Navigation.PopPopupAsync();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Variables
            msg = parameters.GetValue<string>("Msg");
            msgType = parameters.GetValue<int>("Type");

            //General UI Settings
            Msg.Text = msg;
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}
