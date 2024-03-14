using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Services
{
    //request presupuesta

    public class TrackInfo
    {
        public string address_origin { get; set; }
        public string address_number_origin { get; set; }
        public string lat_device { get; set; }
        public string lng_device { get; set; }
        public string lat_origin { get; set; }
        public string lng_origin { get; set; }
        public string lat_destination { get; set; }
        public string lng_destination { get; set; }
        public string address_destination { get; set; }
        public string address_number_destination { get; set; }
    }
    //respónse presupuesta

    public class TrackInfoResponse : ResponseResult
    {
        public string distance { get; set; }
        public string obs { get; set; }
        public string estado { get; set; }
        public string price { get; set; }
    }
}
