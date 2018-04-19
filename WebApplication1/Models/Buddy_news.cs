using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [MetadataType(typeof(newsMetaData))]
    public partial class news
    {
        public class newsMetaData
        {
            [Display(Name = "Article Title")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a title")]
            public string title { get; set; }
            [Display(Name = "Published Date")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a published date")]
            [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date")]
            public System.DateTime pub_date { get; set; }
            [Display(Name = "Write your article here")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter content for your article")]
            public string article_content { get; set; }
            [Display(Name = "Write a brief summary here")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a brief summary")]
            public string article_summary { get; set; }
            [Display(Name = "Author")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter an author")]
            public string author { get; set; }
        }
    }
}