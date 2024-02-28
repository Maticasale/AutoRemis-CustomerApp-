using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Services
{
    public class ResponseResult
    {
        public ServiceType ServiceState { get; set; }

    }
    public enum ServiceType { CheckOut, Invalid, TimeOut, NoConnection, ResponseFailed }
}
