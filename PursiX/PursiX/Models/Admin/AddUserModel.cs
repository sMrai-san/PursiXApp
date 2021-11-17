using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models.Admin
{
    class AddUserModel
    {
        //Login
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; }
        public int? VerificationCode { get; set; }
        public bool Admin { get; set; }
        public int? LoginId { get; set; }
        public bool Confirmed { get; set; }
        

        //UserInfo
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }

        //Is admin logged
        public bool AdminLogged { get; set; }
    }
}
