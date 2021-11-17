using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PursiXMVC.Data
{
    public partial class Events
    {
        public Events()
        {
            EventParticipations = new HashSet<EventParticipations>();
        }

        public int EventId { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja valitse tapahtuman päivämäärä")]
        public DateTime EventDateTime { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä tapahtuman nimi")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä tapahtuman kuvaus")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja ilmoita osallistujien enimmäismäärä")]
        public int? MaxParticipants { get; set; }
        public string Url { get; set; }
        public string AdditionalDetails { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public virtual ICollection<EventParticipations> EventParticipations { get; set; }
    }
}
