using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXApi.Models
{
    public class RegistrationModel
    {
        //Login
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public int? VerificationCode { get; set; }
        public bool Admin { get; set; }
        public bool AdminLogged { get; set; }
        public int? LoginId { get; set; }

        //UserInfo
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
    }
}
