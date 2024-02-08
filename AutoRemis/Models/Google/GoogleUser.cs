using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Google
{
    public class GoogleUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Uri Picture { get; set; }
    }
}
