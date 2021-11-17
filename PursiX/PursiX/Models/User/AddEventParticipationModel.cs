using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models.User
{
    class AddEventParticipationModel
    {
        public int EventId { get; set; }
        public int LoginId { get; set; }
        public bool? Confirmed { get; set; }
        public string AddInfo { get; set; }
    }
}
