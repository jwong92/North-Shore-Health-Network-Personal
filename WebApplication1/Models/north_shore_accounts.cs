//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class north_shore_accounts
    {
        public north_shore_accounts()
        {
            this.donations = new HashSet<donation>();
        }
    
        public int id { get; set; }
        public int user_id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string province_char { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int payment_info { get; set; }
    
        public virtual ICollection<donation> donations { get; set; }
        public virtual payment_information payment_information { get; set; }
        public virtual province province { get; set; }
        public virtual user user { get; set; }
    }
}
