using System;
using System.Collections.Generic;

namespace PursiXMVC.Data
{
    public partial class Login
    {
        public Login()
        {
            EventParticipations = new HashSet<EventParticipations>();
            UserInfo = new HashSet<UserInfo>();
        }

        public int LoginId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public int? VerificationCode { get; set; }
        public bool Admin { get; set; }
        public bool? Confirmed { get; set; }

        public virtual ICollection<EventParticipations> EventParticipations { get; set; }
        public virtual ICollection<UserInfo> UserInfo { get; set; }
    }
}
