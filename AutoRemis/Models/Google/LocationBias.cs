using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AutoRemis.Models.Google
{
    public class LocationBias
    {
        public readonly double latitude;
        public readonly double longitude;
        public readonly int radius;
        public LocationBias(double latitude, double longitude, int radius)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.radius = radius;
        }

        public override string ToString()
        {
            var latFormatted = latitude.ToString(CultureInfo.InvariantCulture);
            var lonFormatted = longitude.ToString(CultureInfo.InvariantCulture);

            return $"&location={latFormatted},{lonFormatted}&radius={radius}";
        }
    }
}
