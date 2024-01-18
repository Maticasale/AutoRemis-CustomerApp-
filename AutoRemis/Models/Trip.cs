using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models
{
    public class Trip
    {
        public string status { get; set; }
        public string name { get; set; }
        public string address_origin { get; set; }
        public string address_number_origin { get; set; }
        public string flats_block { get; set; }
        public string tower { get; set; }
        public string flor { get; set; }
        public string flat { get; set; }
        public string block { get; set; }
        public string lot { get; set; }
        public string country { get; set; }
        public string complex { get; set; }
        public string house { get; set; }
        public string lat_origin { get; set; }
        public string lng_origin { get; set; }
        public string observation { get; set; }
        public string discountCoupon { get; set; }
        public string user { get; set; }
        public string phone { get; set; }
        public string address_destination { get; set; }
        public string address_number_destination { get; set; }
        public string lat_destination { get; set; }
        public string lng_destination { get; set; }
    }

    /// <summary>
    /// Searching: Buscando Viaje
    /// Waiting: Esperando que el movil arribe a su posicion (Aplica aun cuando el movile sta en puerta)
    /// Traveling: Viajando
    /// </summary>
    public enum TripStatus {Searching, Waiting, Delayed, Traveling, Canceled }
}
