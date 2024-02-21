using AutoRemis.Models;
using ImTools;
using Prism.Navigation;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AutoRemis.Views
{
    public partial class AviableUpdatePopUp : PopupPage, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private bool softUpdate;
        private string newVersion;

        public AviableUpdatePopUp(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
            LoadUI();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //Parameters
            softUpdate = parameters.GetValue<bool>("hardUpdate");
            newVersion = parameters.GetValue<string>("newVersion");
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        private void LoadUI()
        {
            imgUpgrade.BackgroundColor = Color.FromHex($"{(softUpdate ? "#FFC021" : "#ff355b")}");
            lblTitle.Text = $"VERSIÓN {(softUpdate ? "DISPONIBLE" : "NECESARIA")}";
            frmVersion.BackgroundColor = Color.FromHex($"{(softUpdate ? "#fff9e9" : "#ffebef")}");
            frmVersion.BorderColor = Color.FromHex($"{(softUpdate ? "#FFC021" : "#ff355b")}");
            lblVersion.Text = $"V {newVersion}";
            lblVersion.TextColor = Color.FromHex($"{(softUpdate ? "#FFC021" : "#ff355b")}");
            lblBody1.Text = $"{(softUpdate ? "Hay una nueva actualización disponible para mejorar tu experiencia. Si bien no es obligatoria, te recomendamos la instalacion para que puedas disfrutar de las últimas mejoras y correcciones de errores." : "Hemos lanzado una nueva actualización de la aplicación que es necesaria para garantizar un funcionamiento óptimo y mantener la compatibilidad con nuestros servicios.")}";
            lblBody2.Text = $"{(softUpdate ? "Puedes actualizar ahora o más tarde desde la PlayStore." : "Lamentablemente no es posible usar esta aplicacion en esta actual versión, por lo que recomentamos dicha instalación")}";
            btnUpdate.BackgroundColor = Color.FromHex($"{(softUpdate ? "#FFC021" : "#ff355b")}");
            btnIgnore.IsVisible = softUpdate;
            if (!softUpdate)
            {
                btnUpdate.CornerRadius = new CornerRadius(0, 0, 0, 0);
                btnUpdate.HorizontalOptions = LayoutOptions.FillAndExpand;
                Grid.SetColumn(btnUpdate, 0);
                Grid.SetColumnSpan(btnUpdate, 2);
            }
        }

        private async void IgnoreClicked(object sender, EventArgs e)
        {
            if (softUpdate) 
                await Navigation.PopPopupAsync();
        }

        private async void UpdateClicked(object sender, EventArgs e)
        {
            //ir a la pagina de la playstore
            string url = "https://play.google.com/store/apps/details?id=com.ubercab&hl=es_AR&gl=US";
            await Browser.OpenAsync(url, BrowserLaunchMode.External);
        }
    }
}
