using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models.Admin
{
    class ModifyEvent
    {
        public int EventId { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? MaxParticipants { get; set; }
        public string Url { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool AdminLogged { get; set; }
    }
}
