using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXMVC.Models
{
    public class RegisterViewModel
    {
        [Key]
        public int Key { get; set; }

        //Login
        [Required(ErrorMessage = "Ole hyvä ja syötä sähköpostiosoite")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Ole hyvä ja syötä kelvollinen sähköpostiosoite")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä salasana")]
        [RegularExpression("^(?=.*?[a-zA-Z])(?=.*?[0-9]).{8,}$", ErrorMessage = "Salasanassa tulee olla vähintään 8 merkkiä ja sen tulee sisältää vähintään 1 kirjain ja 1 numero")]
        public string PassWord { get; set; }

        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Aktivointikoodissa pitää olla 4 numeroa, ole hyvä ja syötä haluttu aktivointikoodi")]
        public int? VerificationCode { get; set; }

        public bool Confirmed { get; set; }

        public bool Admin { get; set; }

        public bool EmailConfirmed { get; set; }

        //UserInfo
        [Required(ErrorMessage = "Ole hyvä ja syötä etunimi")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä sukunimi")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä lähiosoite")]
        [StringLength(200)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä postinumero")]
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Postinumerossa pitää olla 5 numeroa, ole hyvä ja tarkasta postinumero")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä postitoimipaikka")]
        [StringLength(100)]
        public string City { get; set; }

        [Required(ErrorMessage = "Ole hyvä ja syötä puhelinnumero")]
        [RegularExpression("^((([+][s]{0,1})|([0]{2}[s-]{0,1}))([358]{3})([s-]{0,1})|([0]{1}))(([1-9]{1}[0-9]{0,1})([s-]{0,1})([0-9]{2,4})([s-]{0,1})([0-9]{2,4})([s-]{0,1}))([0-9]{0,3}){1}$", ErrorMessage = "Ole hyvä ja syötä kelvollinen puhelinnumero")]
        [StringLength(50)]
        public string Phone { get; set; }

        public bool AdminLogged { get; set; }
    }
}
