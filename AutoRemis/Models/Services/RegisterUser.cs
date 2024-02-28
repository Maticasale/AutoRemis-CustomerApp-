using Newtonsoft.Json;

namespace AutoRemis.Models.Services
{
    //request
    public class RegisterUser
    {
        public string email { get; set; }
        public string fullName { get; set; }
        public string phoneNumber { get; set; }
        public string usrFcb { get; set; }
        public string appVersion { get; set; }
        public string token { get; set; }
    }

    //response
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
