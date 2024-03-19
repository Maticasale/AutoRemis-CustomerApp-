using AutoRemis.Models.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models
{
    public class TripInfo
    {
        public string id_viaje { get; set; }
        public string programed { get; set; }
        public string status { get; set; }
        public string address_origin { get; set; }
        public string address_number_origin { get; set; }
        public string lat_origin { get; set; }
        public string lng_origin { get; set; }
        public string address_destination { get; set; }
        public string address_number_destination { get; set; }
        public string lat_destination { get; set; }
        public string lng_destination { get; set; }
        public string price { get; set; }
    }

    //request viaje
    public class Trip : TripInfo
    {
        public string paymentMethod { get; set; }
        public string carRequested { get; set; }
        public string lat_device { get; set; }
        public string lng_device { get; set; }
        public string name { get; set; }
        public string flats_block { get; set; }
        public string tower { get; set; }
        public string flor { get; set; }
        public string flat { get; set; }
        public string block { get; set; }
        public string lot { get; set; }
        public string country { get; set; }
        public string complex { get; set; }
        public string house { get; set; }
        public string observation { get; set; }
        public string discountCoupon { get; set; }
        public string user { get; set; }
        public string phone { get; set; }
    }

    //response viaje
    public class TripResponse : ResponseResult
    {
        public string estado { get; set; }
        public string id_viaje { get; set; }
        public string obs { get; set; }
    }

    //request estadoviaje y cancelaviaje
    public class TripState
    {
        public string id_viaje { get; set; }
        public string phone { get; set; }
    }

    //response estadoviaje
    public class TripStateResponse : ResponseResult
    {
        public string status { get; set; }
        public List<CarLocation> near_cars { get; set; }
        public string distance { get; set; }
        public string waiting_time { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string driver { get; set; }
        public string urlPic { get; set; }
        public Car car { get; set; }
    }

    //request actualizaviaje
    public class UpdateTrip : TripState
    {
        public string estado { get; set; }
    }

    //request calificarviaje
    public class RateTrip : TripState
    {
        public string comment { get; set; }
        public string rateService { get; set; }
        public string rateDriver { get; set; }
    }
}
