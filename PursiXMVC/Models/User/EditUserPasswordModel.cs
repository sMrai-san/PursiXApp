using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXMVC.Models.User
{
    public class EditUserPasswordModel
    {
        public int LoginId { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä salasana")]
        public string PassWord { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä salasana")]
        [RegularExpression("^(?=.*?[a-zA-Z])(?=.*?[0-9]).{8,}$", ErrorMessage = "Salasanassa tulee olla vähintään 8 merkkiä ja sen tulee sisältää vähintään 1 kirjain ja 1 numero")]
        public string NewPassWord { get; set; }



    }
}
