using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [MetadataType(typeof(payment_informationmetadata))]
    public partial class payment_information
    {
        public class payment_informationmetadata
        {
            [Display(Name = "Payment Information Id")]
            public int id { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your credit card number")]
            [DataType(DataType.CreditCard, ErrorMessage = "Please enter a valid credit card number")]
            [Display(Name = "Credit Card")]
            [Index(IsUnique = true)]
            public string credit_card { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your ccv number")]
            [Display(Name = "CCV")]
            [Index(IsUnique = true)]
            public string ccv { get; set; }
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the expiry date")]
            [DataType(DataType.Date, ErrorMessage = "Plaese enter a valid date")]
            [Display(Name = "Expiry Date")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public System.DateTime exp_date { get; set; }
        }
    }
}