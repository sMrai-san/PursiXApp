using System;
using System.Collections.Generic;

namespace PursiXMVC.Data
{
    public partial class EventParticipations
    {
        public int ParticipationId { get; set; }
        public int EventId { get; set; }
        public int LoginId { get; set; }
        public bool? Confirmed { get; set; }
        public string AddInfo { get; set; }

        public virtual Events Event { get; set; }
        public virtual Login Login { get; set; }
    }
}
