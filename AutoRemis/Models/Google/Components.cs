using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Google
{
    public class Components
    {
        private string components;

        public Components(string components)
        {
            this.components = components;
        }
        public override string ToString() => $"&components={components}";
    }
}
