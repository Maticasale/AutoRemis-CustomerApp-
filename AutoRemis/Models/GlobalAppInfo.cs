using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models
{
    public class GlobalAppInfo
    {
        public string GlobalApiKey { get; set; }
        public string FirebaseToken { get; set; }
        public User UserInfo { get; set; }
    }
}
