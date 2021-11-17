using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PursiXMVC.Models.User
{
    public class EditUserInfoModel
    {
        public int? LoginId { get; set; }
        public string Email { get; set; }

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

        public bool isLogged { get; set; }



    }
}
