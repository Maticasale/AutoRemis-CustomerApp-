using AutoRemis.Constants;
using AutoRemis.Helpers;
using AutoRemis.Models.Services;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AutoRemis.Services
{
    public static class CarService
    {
        private static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        private static HttpClient client;
        private static CancellationTokenSource ct;        
        static private AppStateManager user { get; set; }

        static CarService() { client = new HttpClient(); }

        public static async Task<NearCarresponse> GetNearCars(NearCar user)
        {
            NearCarresponse _response = new NearCarresponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<NearCarresponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(user);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/movilescercanos.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response.NearCars = JsonConvert.DeserializeObject<List<CarLocation>>(result);

                        _response.ServiceState = _response.NearCars.Count != 0 ? ServiceType.CheckOut : ServiceType.Invalid;
                    }
                    else
                        _response.ServiceState = ServiceType.ResponseFailed;
                }
                catch (OperationCanceledException)
                {
                    _response.ServiceState = ServiceType.TimeOut;
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
