using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models.Admin
{
    class EventsAndParticipationsCombinedModel
    {
        public int ParticipationId { get; set; }
        public int EventId { get; set; }
        public int LoginId { get; set; }
        public bool? Confirmed { get; set; }
        public string AddInfo { get; set; }

        public DateTime? EventDateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? JoinedParticipants { get; set; }
        public int? MaxParticipants { get; set; }
        public string Url { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int UserInfoId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int? PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public bool AdminLogged { get; set; }
    }
}
