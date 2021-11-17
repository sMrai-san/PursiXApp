using System;
using System.Collections.Generic;

namespace PursiXMVC.Data
{
    public partial class UserInfo
    {
        public int UserInfoId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? LoginId { get; set; }

        public virtual Login Login { get; set; }
    }
}
