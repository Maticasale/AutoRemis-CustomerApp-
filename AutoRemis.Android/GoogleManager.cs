using System;
using Android.Accounts;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using AutoRemis.Droid;
using AutoRemis.Models.Google;
using Xamarin.Forms;

[assembly: Dependency(typeof(GoogleManager))]
namespace AutoRemis.Droid
{
    public class GoogleManager : Java.Lang.Object, IGoogleManager, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        public Action<GoogleUser, string> _onLoginComplete;
        public static GoogleApiClient _googleApiClient { get; set; }
        public static GoogleManager Instance { get; private set; }
        Context _context;

        public GoogleManager()
        {
            _context = Android.App.Application.Context;
            Instance = this;
        }

        public void Login(Action<GoogleUser, string> onLoginComplete)
        {
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn).RequestEmail().Build();
            _googleApiClient = new GoogleApiClient.Builder((_context).ApplicationContext)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .AddScope(new Scope(Scopes.Profile))
                .Build();

            _onLoginComplete = onLoginComplete;
            Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(_googleApiClient);
            ((MainActivity)Forms.Context).StartActivityForResult(signInIntent, 1);
            _googleApiClient.Connect();
        }

        public void Logout()
        {
            var gsoBuilder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn).RequestEmail();

            if (_googleApiClient != null && _googleApiClient.IsConnected)
            {
                GoogleSignIn.GetClient(_context, gsoBuilder.Build())?.SignOut();
                _googleApiClient.Disconnect();
            }
        }

        public void OnAuthCompleted(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                GoogleSignInAccount account = result.SignInAccount;
                _onLoginComplete?.Invoke(new GoogleUser()
                {
                    FirstName = account.GivenName, 
                    LastName = account.FamilyName,
                    FullName = account.DisplayName,
                    Email = account.Email,
                    Picture = new Uri(account.PhotoUrl?.ToString() ?? "https://autisticdating.net/imgs/profile-placeholder.jpg")
                }, string.Empty);
            }
            else
                _onLoginComplete?.Invoke(null, $"Error en la autenticación: {result.Status.StatusMessage}");
        }

        public void OnConnected(Bundle connectionHint) { }

        public void OnConnectionSuspended(int cause) => _onLoginComplete?.Invoke(null, "Caanceled!");

        public void OnConnectionFailed(ConnectionResult result) => _onLoginComplete?.Invoke(null, result.ErrorMessage);
    }
}