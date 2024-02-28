using AutoRemis.Helpers;
using AutoRemis.Models.Services;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;

namespace AutoRemis.Services
{
    public static class TripService
    {
        private static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        private static HttpClient client;
        private static CancellationTokenSource ct;
        static private AppStateManager user { get; set; }
        static TripService() { client = new HttpClient(); }


    }
}
