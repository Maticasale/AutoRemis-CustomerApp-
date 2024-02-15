using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AutoRemis.Helpers
{
    public static class LocationHelper
    {
        public static async Task<LocationResponse> GetLocation()
        {
            try
            {
                Location loc = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));

                if (loc != null && (loc.Latitude != 0 && loc.Longitude != 0))
                    return new LocationResponse() { Location = loc, Status = LocationStatus.OK };
                else
                    return new LocationResponse() { Location = null, Status = LocationStatus.Unknown };
            }
            catch (Exception e)
            {
                return new LocationResponse() { Location = null, Status = LocationStatus.Exception };
            }
        }

        public class LocationResponse
        {
            public LocationStatus Status { get; set; }
            public Location Location { get; set; }
        }
        public enum LocationStatus { OK, Unknown, Exception }
    }
}
