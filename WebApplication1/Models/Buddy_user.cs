using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [MetadataType(typeof(userMetaData))]
    public partial class user
    {
        public class userMetaData
        {
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a username")]
            [Display(Name = "Enter a Username")]
            public string username { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a password")]
            [Display(Name = "Enter a Password")]
            [DataType(DataType.Password)]
            public string password { get; set; }
            [Display(Name = "Role")]
            public string user_role { get; set; }
        }
    }
}