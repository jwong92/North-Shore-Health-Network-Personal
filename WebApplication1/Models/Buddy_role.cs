using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [MetadataType(typeof(rolesMetaData))]
    public partial class role
    {
        public class rolesMetaData
        {
            [Display(Name = "Role")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a user role")]
            public string role_code { get; set; }
        }
    }
}