using Prism.Navigation;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms;

namespace AutoRemis.Views
{
    public partial class Trip_InformationPopUp : PopupPage, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private string msg;
        public Trip_InformationPopUp(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        private void OnBackgroundClicked(object sender, EventArgs e) => Navigation.PopPopupAsync();

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Variables
            msg = parameters.GetValue<string>("Msg");

            //General UI Settings
            Msg.Text = msg;
        }

        private void OkClicked(object sender, EventArgs e) => Navigation.PopPopupAsync();
    }
}




