using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXApi.Models
{
    public class UserInfoEdit
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

        public bool isLogged { get; set; }
    }
}
