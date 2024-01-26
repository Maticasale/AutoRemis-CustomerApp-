using AutoRemis.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;
using static AutoRemis.Helpers.AppStateManager;

namespace AutoRemis.Helpers
{
    public static class LocationHelper
    {
        public enum LocationStatus { OK, Unknown, Exception }
        public static async Task<LocationStatus> GetLocation(UserStatus Newstatus)
        {
            try
            {
                var loc = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));

                if (loc != null && (loc.Latitude != 0 && loc.Longitude != 0))
                {
                    User user = GetUser();
                    user.lastKnownPosition = new Position(loc.Latitude, loc.Longitude);
                    user.Status = Newstatus;

                    UpdateUser(user);

                    return LocationStatus.OK;
                }
                else
                    return LocationStatus.Unknown;
                    
            }
            catch (Exception ex)
            {
                return LocationStatus.Exception;
            }
        }
    }
}
