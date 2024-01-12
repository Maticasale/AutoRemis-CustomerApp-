using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AutoRemis.Helpers
{
    public static class SBManager
    {
        //        public static void ShowSnackbar(string errorName, string errorMsg, int sec, SoundTools.SoundType type)
        //        {
        //            Device.BeginInvokeOnMainThread(async () =>
        //            {
        //                OnIsBusy(false);

        //                Msg.Text = errorMsg;
        //                MsgName.Text = errorName;

        //                switch (type)
        //                {
        //                    case SoundTools.SoundType.Error:
        //                        MsgItem.Source = "ErrorIcon2.png";
        //                        CancellBox.Border.Color = Color.FromHex("#f44336");
        //                        MsgName.TextColor = Color.FromHex("#f44336");
        //                        SoundTools.PlaySound(SoundTools.SoundType.Alert);
        //                        break;
        //                    case SoundTools.SoundType.Alert:
        //                        MsgItem.Source = "WarningIcon.png";
        //                        CancellBox.Border.Color = Color.Yellow;
        //                        MsgName.TextColor = Color.Yellow;
        //                        SoundTools.PlaySound(SoundTools.SoundType.Alert);
        //                        break;
        //                    case SoundTools.SoundType.Success:
        //                        MsgItem.Source = "SuccessIcon.png";
        //                        CancellBox.Border.Color = Color.Green;
        //                        MsgName.TextColor = Color.Green;
        //                        SoundTools.PlaySound(SoundTools.SoundType.Success);
        //                        break;
        //                }

        //                await Task.Delay(TimeSpan.FromSeconds(1));
        //                await Task.WhenAll(CancellBox.TranslateTo(0, 0, 400, easing: Easing.SinIn));
        //                await Task.Delay(TimeSpan.FromSeconds(sec));
        //                await Task.WhenAll(CancellBox.TranslateTo(0, 250, 400, easing: Easing.SinIn));

        //                SoundTools.StopCurrentSound();
        //            });
        //        }
    }
}
