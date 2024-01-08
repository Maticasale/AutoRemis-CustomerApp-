using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRemis.Models.Google
{
    public enum PlaceType
    {
        ///<summary>Get all place types</summary>
        All,
        ///<summary>Get geocode place types</summary>
        Geocode,
        ///<summary>Get address place types</summary>
        Address,
        ///<summary>Get establishment place types</summary>
        Establishment,
        ///<summary>Get regional place types</summary>
        Regions,
        ///<summary>Get city place types</summary>
        Cities
    }
}
