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

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Parameters
            msg = parameters.GetValue<string>("Msg");

            LoadUI();
        }

        private void LoadUI()
        {
            //General UI Settings
            Msg.Text = msg;
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        private void OnBackgroundClicked(object sender, EventArgs e) => Navigation.PopPopupAsync();

        private void OkClicked(object sender, EventArgs e) => Navigation.PopPopupAsync();
    }
}




