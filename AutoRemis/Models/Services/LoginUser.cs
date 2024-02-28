using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Services
{
    //request
    public class LoginUser
    {
        public string tipo { get; set; }
        public string id { get; set; }
        public string appVersion { get; set; }
        public string token { get; set; }
    }

    //response 
    public class LoginUserResponse : ResponseResult
    {
        public string estado { get; set; }
        public string testCase { get; set; }
        public string modoViaje { get; set; }
        public string wsNumber { get; set; }
        public string okZona { get; set; }
        public string okVersion { get; set; }
        public string apiKey { get; set; }
        public string obs { get; set; }
        public TripInfo tripInfo { get; set; }
    }
}
