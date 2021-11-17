using System;
using System.Collections.Generic;
using System.Text;

namespace PursiX.Models.User
{
    class EditLoginModel
    {
        public int LoginId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public string NewPassWord { get; set; }
    }
}
