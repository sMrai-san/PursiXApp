using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models
{
    class Login
    {
        public int LoginID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public int? VerificationCode { get; set; }
        public bool Admin { get; set; }
    }
}
