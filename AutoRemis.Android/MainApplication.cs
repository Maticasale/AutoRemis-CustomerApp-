using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using AutoRemis.Constants;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;

namespace AutoRemis.Droid
{
    [MetaData("com.google.android.maps.v2.API_KEY", Value = AppConstants.GoogleMapsApiKey)]
    [Application(Theme = "@style/MainTheme")]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();
            Xamarin.Essentials.Platform.Init(this);
            CrossCurrentActivity.Current.Init(this);

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }

            //If debug you should reset the token each time.
            try
            {
                #if DEBUG
                FirebasePushNotificationManager.Initialize(this, true);
                #else
                FirebasePushNotificationManager.Initialize(this, false);
                #endif
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("ERROR AL REGISTRAR EL TOKEN");
            }
        }
    }
}
