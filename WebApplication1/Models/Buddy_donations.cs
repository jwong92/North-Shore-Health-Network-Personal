using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    [MetadataType(typeof(donationmetadata))]
    public partial class donation
    {
        class donationmetadata
        {
            [Display(Name = "Donor ID")]
            public int id { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter an amount")]
            [DataType(DataType.Currency, ErrorMessage = "Please enter a valid amount")]
            [Display(Name = "Donation Amount")]
            public decimal amount { get; set; }
            [Display(Name = "Account ID")]
            public int account_id { get; set; }
        }
    }
}