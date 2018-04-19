using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    [MetadataType(typeof(north_shore_accountsmetadata))]
    public partial class north_shore_accounts
    {
        public class north_shore_accountsmetadata
        {
            [Display(Name = "Account ID")]
            public int id { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your first name")]
            [DataType(DataType.Text, ErrorMessage = "Please ensure you enter a valid first name")]
            [Display(Name = "First Name")]
            public string fname { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your last name")]
            [DataType(DataType.Text, ErrorMessage = "Please ensure you enter a valid last name")]
            [Display(Name = "Last Name")]
            public string lname { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your address")]
            [Display(Name = "Address")]
            public string address { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your city of residence")]
            [Display(Name = "City")]
            public string city { get; set; }
            [Display(Name = "Province")]
            public string province_char { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email")]
            public string email { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone Number")]
            public string phone { get; set; }
        }
    }
}