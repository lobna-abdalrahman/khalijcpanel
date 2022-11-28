using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpanel.Models
{
 
    public class CustomerRegBankinfo
    {
     
        public List<SelectListItem> Branches { get; set; }
        public List<SelectListItem> AccTypes { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public List<SelectListItem> catgories { get; set; }
        public List<channel> Channels { get; set; }
       
        [Display(Name = "Customer Branch")]
        public  string Branch { get; set; }
        //[Required]
        [Display(Name = "Account Currency")]
        public string Currency { get; set; }
        //[Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
        [Required]
        [Display(Name = "Account Number")]


 
        public string AccountNumber { get; set; }


        [Required]
        [Display(Name = "Customer's username")]
        public string UserLog { get; set; }
        [Display(Name = "Customer Card")]
        public String CustomerCard { get; set; }

        [Required]
        public String BranchCode { get; set; }

        [Required]
        public String AccountTypecode { get; set; }

        [Required]
        public String CurrencyCode { get; set; }


        public string CustomerName { get; set; }
     
        public String CustomerID { get; set; }
        [Required]
        public String CategoryCode { get; set; }
         [Display(Name="Category")]
        public String category { get; set; }
         public String[] SelectedChannelsID { get; set; }
         [Display(Name = "Channel")]
         public String Channel { get; set; }
         public List<channel> SelectedChannels { get; set; }
     
    }
    public class CustomerRegBankinfo2
    {
        
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }
        [Display(Name = "Customer ID")]
        public String CustomerID { get; set; }
    }

   public class addaccount
   {
        [Display(Name = "Customer Branch")]
        public  string Branch { get; set; }
        //[Required]
        [Display(Name = "Account Currency")]
        public string Currency { get; set; }
        //[Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [Display(Name = "Account Number")]
        public string AccountNumbercomplete { get; set; }
       
        public int AccountID { get; set; }
        public bool IsSelected { get; set; }
   }
   public class accountsresult
   {

       public List<addaccount> accountSelected { get; set; }
       public int[] acctIds { get; set; }
   }

   public class account
   {
       [Required]
       [Display(Name = "Customer Account")]
       public string Account { get; set; }
   }


   public class pendingacts
   {
        [Display(Name = "Customer ID")]
       public string USER_ID { get; set; }
       [Display(Name = "Customer Name")]
       public string USER_NAME { get; set; }
       [Display(Name = "Customer Account")]
       public string DEF_ACC { get; set; }
        [Display(Name = "Customer New Account")]
       public string ACC_NO { get; set; }
      
   }
   public class actAuthorizationinfo
   {
       [Display(Name = "Customer Branch")]
       public string Branch { get; set; }
       //[Required]
       [Display(Name = "Account Currency")]
       public string Currency { get; set; }
       //[Required]
       [Display(Name = "Customer Account Type")]
       public string AccountType { get; set; }
       [Display(Name = "Customer Name")]
       public String Customername { get; set; }
       [Display(Name = "Customer Account")]
       public String Customeraccount { get; set; }
       [Display(Name = "Customer ID")]
       public String CustomerID { get; set; }
       
       [Display(Name = "Reject Reason")]
       public string RejectReason { get; set; }
       public string userid { get; set; }
       public string authsts { get; set; }
       public string rjtsts { get; set; }
       public string completeact { get; set; }
   }
 
 
}