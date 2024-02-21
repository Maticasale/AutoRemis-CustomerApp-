﻿using AutoRemis.Constants;
using AutoRemis.Helpers;
using AutoRemis.Models.Services;
using Newtonsoft.Json;
using Polly;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AutoRemis.Services
{
    public static class Auth
    {
        private static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
        private static HttpClient client;
        private static CancellationTokenSource ct;

        static private AppStateManager user { get; set; }
        static Auth() { client = new HttpClient(); }

        
        public static async Task<LoginUserResponse> Login(LoginUser user)
        {
            LoginUserResponse _response = new LoginUserResponse();

            if (!IsConnected)
            {
                _response.responseServiceType = ResponseType.NoConnection;
                return _response;
            }

            ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(5));

            var retryPolicy = Policy.Handle<Exception>().OrResult<LoginUserResponse>(r => r?.responseServiceType != ResponseType.OK).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1), (ex, time) => { });

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

                        _response.responseServiceType = ResponseType.OK;
                    }
                    else
                        _response.responseServiceType = ResponseType.ResponseFailed;
                }
                catch (Exception)
                {
                    _response.responseServiceType = ResponseType.ResponseFailed;
                }
                return _response;
            });
            return _response;
        }
    }
}
