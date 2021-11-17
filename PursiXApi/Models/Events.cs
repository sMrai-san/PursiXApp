using System;
using System.Collections.Generic;

namespace PursiXApi.Models
{
    public partial class Events
    {
        public Events()
        {
            EventParticipations = new HashSet<EventParticipations>();
        }

        public int EventId { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? MaxParticipants { get; set; }
        public string Url { get; set; }
        public string AdditionalDetails { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public virtual ICollection<EventParticipations> EventParticipations { get; set; }
    }
}
