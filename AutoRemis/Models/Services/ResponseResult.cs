using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Services
{
    public class ResponseResult
    {
        public ResponseType responseServiceType { get; set; }
    }
    public enum ResponseType { OK, TimeOut, NoConnection, ResponseFailed }
}
