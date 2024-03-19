using AutoRemis.Helpers;
using AutoRemis.Models.Services;
using Newtonsoft.Json;
using Polly;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;
using AutoRemis.Constants;
using AutoRemis.Models;
using System.Collections.Generic;

namespace AutoRemis.Services
{
    public static class TripService
    {
        private static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        private static HttpClient client;
        private static CancellationTokenSource ct;
        static private AppStateManager user { get; set; }
        static TripService() { client = new HttpClient(); }

        public static async Task<TrackInfoResponse> GetTrackInfo(TrackInfo track)
        {
            TrackInfoResponse _response = new TrackInfoResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<TrackInfoResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(track);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/presupuesta.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<TrackInfoResponse>(result);

                        _response.ServiceState = _response.estado == "OK" ? ServiceType.CheckOut : ServiceType.Invalid;
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

        public static async Task<TripResponse> StartTrip(Trip trip)
        {
            TripResponse _response = new TripResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<TripResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(trip);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/viaje.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<TripResponse>(result);

                        _response.ServiceState = (_response.estado == "OK" && _response.id_viaje != "0") ? ServiceType.CheckOut : ServiceType.Invalid;
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

        public static async Task<TripStateResponse> GetTripState(TripState trip)
        {
            TripStateResponse _response = new TripStateResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<TripStateResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(trip);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/estadoviaje.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<TripStateResponse>(result);

                        _response.ServiceState = ServiceType.CheckOut;
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

        public static async Task<BoolResponse> CancelTrip(TripState trip)
        {
            BoolResponse _response = new BoolResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<BoolResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(trip);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/cancelaviaje.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<BoolResponse>(result);

                        _response.ServiceState = ServiceType.CheckOut;
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

        public static async Task<BoolResponse> UpdateTripStatus(UpdateTrip newStatus)
        {
            BoolResponse _response = new BoolResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<BoolResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(newStatus);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/actualizaviaje.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<BoolResponse>(result);

                        _response.ServiceState = ServiceType.CheckOut;
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

        public static async Task<BoolResponse> RateTrip(RateTrip newStatus)
        {
            BoolResponse _response = new BoolResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<BoolResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(newStatus);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/calificarviaje.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<BoolResponse>(result);

                        _response.ServiceState = ServiceType.CheckOut;
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
