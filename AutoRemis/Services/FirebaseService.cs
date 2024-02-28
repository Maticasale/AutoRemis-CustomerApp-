using AutoRemis.Helpers;
using AutoRemis.Models.Services;
using Polly;
using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using Xamarin.Essentials;
using AutoRemis.Constants;
using System.Threading.Tasks;
using static AutoRemis.Helpers.AppStateManager;

namespace AutoRemis.Services
{
    public static class FirebaseService
    {
        private static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
        private static HttpClient client;
        private static CancellationTokenSource ct;

        static private AppStateManager user { get; set; }
        static FirebaseService() { client = new HttpClient(); }


        public static async Task<BoolResponse> FCMReplier(string idFCM)
        {
            BoolResponse _response = new BoolResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(5));

            var retryPolicy = Policy.Handle<Exception>().OrResult<BoolResponse>(r => r.ServiceState != ServiceType.Invalid || r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1), (ex, time) => { });

            var json = JsonConvert.SerializeObject(user);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/rcvfcm.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<BoolResponse>(result);

                        _response.ServiceState = ServiceType.CheckOut;    
                    }
                    else
                        _response.ServiceState = ServiceType.ResponseFailed;
                }
                catch (Exception)
                {
                    _response.ServiceState = ServiceType.ResponseFailed;
                }
                return _response;
            });
            return _response;
        }
        
        public static async Task<LoginUserResponse> RefreshToken(LoginUser user)
        {
            LoginUserResponse _response = new LoginUserResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(5));

            var retryPolicy = Policy.Handle<Exception>().OrResult<LoginUserResponse>(r => r.ServiceState != ServiceType.Invalid || r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1), (ex, time) => { });

            var json = JsonConvert.SerializeObject(user);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/actualizar_token.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<LoginUserResponse>(result);

                        _response.ServiceState = ServiceType.CheckOut;
                    }
                    else
                        _response.ServiceState = ServiceType.ResponseFailed;
                }
                catch (Exception)
                {
                    _response.ServiceState = ServiceType.ResponseFailed;
                }
                return _response;
            });
            return _response;
        }
    }
}