using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Services
{
    //request reenvio_codigo
    public class BasicUserInfo
    {
        public string email { get; set; }
        public string phoneNumber { get; set; }
    }

    //request reenvio_codigo
    public class BasicUserInfoResponse : ResponseResult
    {
        public string smsToken { get; set; }
        public string estado { get; set; }
    }



    //request registrar_notificaciones
    public class LoginUser
    {
        public string tipo { get; set; }
        public string id { get; set; }
        public string appVersion { get; set; }
        public string token { get; set; }
    }

    //response registrar_notificaciones
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

    //request inicio_registro
    public class RegisterUser : BasicUserInfo
    {
        public string fullName { get; set; }
        public string usrFcb { get; set; }
        public string appVersion { get; set; }
        public string token { get; set; }
    }

    //response inicio_registro
    public class RegisterUserResponse : ResponseResult
    {
        public string estado { get; set; }
        public string okZona { get; set; }
        public string okVersion { get; set; }
        public string ApiKey { get; set; }
        public string smsToken { get; set; }
        public string wsNumber { get; set; }
        public string obs { get; set; }
    }


}
