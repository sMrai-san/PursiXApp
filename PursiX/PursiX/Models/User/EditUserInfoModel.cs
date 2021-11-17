﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models.User
{
    class EditUserInfoModel
    {
        public int? LoginId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }

        public bool isLogged { get; set; }
    }
}
