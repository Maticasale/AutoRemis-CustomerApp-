using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Services
{
    //request movilescercanos
    public class NearCar
    {
        public string lat_origin { get; set; }
        public string lng_origin { get; set; }
        public string lat_device { get; set; }
        public string lng_device { get; set; }
    }

    //respónse movilescercanos
    public class NearCarResponse : ResponseResult
    {
        public List<CarLocation> NearCars { get; set; }
    }

    public class CarLocation
    {
        public string lng { get; set; }
        public string lat { get; set; }
        public int id_movil { get; set; }
        public string tipo { get; set; }
    }

    public class Car
    {
        public string brand { get; set; }
        public string model { get; set; }
        public string domain { get; set; }
        public string movil { get; set; }
        public string tipo { get; set; }
        public string has_application { get; set; }
    }
}
