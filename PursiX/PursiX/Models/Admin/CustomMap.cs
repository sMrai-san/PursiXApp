using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace PursiX.Models.Admin
{
    class CustomMap : Map
    {
        public List<LocationPin> CustomPins { get; set; }
    }
}
