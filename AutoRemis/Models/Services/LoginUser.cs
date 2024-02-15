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
        public int okZona { get; set; } //esta se convierte en string en el futuro
        public string modoViaje { get; set; }
        public string ApiKey { get; set; }
        public string wsNumber { get; set; }
        public string obs { get; set; }
    }
}
