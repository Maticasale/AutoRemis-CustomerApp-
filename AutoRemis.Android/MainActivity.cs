using Android.Content;
using System.Globalization;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
//using Plugin.FirebasePushNotification;
using Prism;
using Prism.Ioc;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using AutoRemis.Constants;
using Android.Gms.Common;
using Xamarin.Essentials;
using Android.Util;
using Firebase.Messaging;
using Xamarin.Forms;
using AutoRemis.Interfaces;
using AutoRemis.Droid.Services;

namespace AutoRemis.Droid
{
    [MetaData("com.google.android.maps.v2.API_KEY", Value = "AIzaSyAA4fS33bAgNtHXOOA_19ODwmHK3W0cfGQ")]

    [Activity(Theme = "@style/MainTheme", Icon = "@mipmap/ic_launcher", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this);
            Firebase.FirebaseApp.InitializeApp(this);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);
            DependencyService.Register<IFirebaseManager, FirebaseManager>(); 

            this.SetLocale();

            Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new AndroidInitializer()));

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                GoogleManager.Instance.OnAuthCompleted(result);
            }
        }

        private void OpenLocationSettings()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            StartActivity(intent);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void SetLocale()
        {
            CultureInfo ci = new CultureInfo("en-US");

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry) { }
    }
}