using AutoRemis.Helpers;
using System.Threading.Tasks;
using System;
using Xamarin.Forms;

namespace AutoRemis.Views
{
    public partial class Trip_FinishedPage : ContentPage
    {
        public Trip_FinishedPage()
        {
            InitializeComponent();
        }

        private void OkClicked(object sender, EventArgs e)
        {

        }

        private void NOClicked(object sender, EventArgs e)
        {

        }

        private void RiseErrorMsg(string title, string msg, int time, SoundHelper.SoundType type)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //stkOptions.IsVisible = false;

                Title.Text = title;
                Msg.Text = msg;

                switch (type)
                {
                    case SoundHelper.SoundType.Error:
                        imgItem.Source = "iconError.png";
                        CancellBox.BorderColor = Color.FromHex("#ff355b");
                        CancellBox.BackgroundColor = Color.FromHex("#fffbfc");
                        Title.TextColor = Color.FromHex("#ff355b");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Alert:
                        imgItem.Source = "iconWarning.png";
                        CancellBox.BorderColor = Color.FromHex("#FFC021");
                        CancellBox.BackgroundColor = Color.FromHex("#fffefb");
                        Title.TextColor = Color.FromHex("#FFC021");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Alert);
                        break;
                    case SoundHelper.SoundType.Success:
                        imgItem.Source = "iconSuccess.png";
                        CancellBox.BorderColor = Color.FromHex("#47D764");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfefc");
                        Title.TextColor = Color.FromHex("#47D764");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Success);
                        break;
                    case SoundHelper.SoundType.Message:
                        imgItem.Source = "iconInfo.png";
                        CancellBox.BorderColor = Color.FromHex("#2F86EB");
                        CancellBox.BackgroundColor = Color.FromHex("#fbfdff");
                        Title.TextColor = Color.FromHex("#2F86EB");
                        SoundHelper.PlaySound(SoundHelper.SoundType.Message);
                        break;
                }

                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
                await Task.Delay(TimeSpan.FromSeconds(time));
                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

                SoundHelper.StopCurrentSound();

                //stkOptions.IsVisible = true;
            });
        }

    }
}
