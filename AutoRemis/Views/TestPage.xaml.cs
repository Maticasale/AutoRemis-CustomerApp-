using AutoRemis.Models.Google;
using AutoRemis.Services;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace AutoRemis.Views
{
    public partial class TestPage : ContentPage
    {
        private string LatOrg, LngOrg, AdressOrg, NumberOrg;

        private string LatDst, LngDst, AdressDst, NumberDst;

        private bool SearchBarsFoucsed = false;
        private bool AutoFillEnable = true;

        string EntryFocused;
        bool ItemSellected;
        public TestPage()
        {
            InitializeComponent();
        }



        private void OkClicked(object sender, EventArgs e)
        {

        }

        private void CancelClicked(object sender, EventArgs e)
        {

        }

        private void GoogleClicked(object sender, EventArgs e)
        {

        }

        private void PhoneClicked(object sender, EventArgs e)
        {

        }

        private void MailClicked(object sender, EventArgs e)
        {

        }     
    }
}
