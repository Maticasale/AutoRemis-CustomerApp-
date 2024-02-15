using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace AutoRemis.Helpers
{
    public static class ConnectionHelper
    {
        public static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
