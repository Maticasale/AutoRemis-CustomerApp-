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

namespace AutoRemis.Services
{
    public static class AuthService
    {
        private static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
        private static HttpClient client;
        private static CancellationTokenSource ct;

        static private AppStateManager user { get; set; }
        static AuthService() { client = new HttpClient(); }


        public static async Task<LoginUserResponse> Login(LoginUser user)
        {
            LoginUserResponse _response = new LoginUserResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(5));

            var retryPolicy = Policy.Handle<Exception>().OrResult<LoginUserResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1), (ex, time) => { });

            var json = JsonConvert.SerializeObject(user);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/registrar_notificaciones.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<LoginUserResponse>(result);

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

        public static async Task<RegisterUserResponse> Register(RegisterUser user)
        {
            RegisterUserResponse _response = new RegisterUserResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<RegisterUserResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(user);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/inicio_registro.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<RegisterUserResponse>(result);

                        _response.ServiceState = (_response.estado == "OK" && _response.okZona == "1") ? ServiceType.CheckOut : ServiceType.Invalid;
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

        public static async Task<BasicUserInfoResponse> SendSmsToken(BasicUserInfo info)
        {
            BasicUserInfoResponse _response = new BasicUserInfoResponse();

            if (!IsConnected)
            {
                _response.ServiceState = ServiceType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(30));

            var retryPolicy = Policy.Handle<Exception>().OrResult<BasicUserInfoResponse>(r => r.ServiceState != ServiceType.Invalid && r.ServiceState != ServiceType.CheckOut).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(3), (ex, time) => { });

            var json = JsonConvert.SerializeObject(user);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var response = await client.PostAsync($"{AppConstants.ApisUrl}/reenvio_codigo.php", data, ct.Token);

                    if (response.IsSuccessStatusCode && response != null)
                    {
                        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _response = JsonConvert.DeserializeObject<BasicUserInfoResponse>(result);

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
    }
}
