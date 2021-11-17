using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXApi.Models
{
    public class EventParticipationsConfirm
    {
        public int ParticipationId { get; set; }
        public int EventId { get; set; }
        public int LoginId { get; set; }
        public bool? Confirmed { get; set; }
        public string AddInfo { get; set; }

        public bool? AdminLogged { get; set; }
    }
}
