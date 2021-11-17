using PursiXMVC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXMVC.Models
{
    public class EventListModel
    {
            public IEnumerable<Events> EventsList { get; set; }
            public IEnumerable<EventParticipations> ParticipationList { get; set; }

    }
}
