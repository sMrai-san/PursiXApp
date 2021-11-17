using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXApi.Models
{
    public class EditLoginModel
    {
        public int LoginId { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public string NewPassWord { get; set; }
        public bool AdminLogged { get; set; }
    }
}
