using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models
{
    class EventParticipations
    {
        public int ParticipationId { get; set; }
        public int EventId { get; set; }
        public int LoginId { get; set; }
        public bool? Confirmed { get; set; }
        public string AddInfo { get; set; }
    }
}
