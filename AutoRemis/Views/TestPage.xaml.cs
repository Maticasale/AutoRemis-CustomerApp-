using AutoRemis.Models.Google;
using AutoRemis.Services;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using AutoRemis.Models;
using Prism.Navigation;
using AutoRemis.Helpers;

namespace AutoRemis.Views
{
    public partial class TestPage : ContentPage
    {
        User user;
        public TestPage()
        {
            InitializeComponent();
            user = AppStateManager.GetUser();

        }
    }
}
