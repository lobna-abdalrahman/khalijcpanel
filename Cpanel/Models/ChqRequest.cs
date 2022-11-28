using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Cpanel.Models
{
    public class ChqRequest
    {
        public int request_id { get; set; }
        [Display(Name = "Customer Account")]
        public string accountmap { get; set; }
        [Display(Name = "Customer Name")]
        public string name { get; set; }

        [Display(Name = "Book Size")]
        public string booksize { get; set; }
        [Display(Name = "Request Date")]
        public string date { get; set; }

    }
}