using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models
{
    public class FirebaseMessage
    {
        public string cuerpo { get; set; }
        public string de { get; set; }
        public string tipo { get; set; }
        public string fecha { get; set; }
        public string idMsg { get; set; }
        public string idFCM { get; set; }
    }
}
