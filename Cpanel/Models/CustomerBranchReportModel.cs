using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpanel.Models
{
    public class CustomerBranshReportModel
    {
        public List<SelectListItem> Branches { get; set; }
        /*public List<SelectListItem> AccTypes { get; set; }*/
        /*public List<SelectListItem> Currencies { get; set; }*/
        public List<SelectListItem> catgories { get; set; }
        public List<SelectListItem> CustomerStatus { get; set; }

        [Display(Name = "From")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime FromDate { get; set; }

        [Display(Name = "To")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ToDate { get; set; }



        public string CustomerName { get; set; }

        [Display(Name = "Customer ID")]
        public String CustomerID { get; set; }

        [Display(Name = "Customer Branch")]
        public string Branch { get; set; }

        [Display(Name = "Customer Type")]
        public string CustomerType { get; set; }

        [Display(Name = "Customer Status")]
        public string CustStatus { get; set; }

        public String BranchCode { get; set; }
        public String CategoryCode { get; set; }

        public String StatusCode { get; set; }
        [Display(Name = "Account Currency")]
        public string Currency { get; set; }


        [Display(Name = "Account Type")]
        public string AccountType { get; set; }


        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Regestration Date")]
        public string RegDate { get; set; }
    }
}