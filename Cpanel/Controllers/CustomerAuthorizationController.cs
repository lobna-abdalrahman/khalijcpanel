using Cpanel.Models;
using Cpanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpanel.Controllers
{
    public class CustomerAuthorizationController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /CustomerAuth/
        public ActionResult CustomerAuthorization()
        {
            if (Session["result"] != null)
            {
                ViewBag.SuccessMessage = Session["result"].ToString();
                Session["result"] = null;
            }
            
            //Session["bracode"] = "000";
            String branchcode = Session["bracode"].ToString();

            List<CustomerAuthorization> customer = new List<CustomerAuthorization>();
            customer = ds.PendingCustomer(branchcode);

            return View(customer);
        }
        public ActionResult Details(int id)
        {
            CustomerAuthorizationinfo model = new CustomerAuthorizationinfo();
            Session["user"] = id;
            List<CustomerAuthorizationinfo> customer = new List<CustomerAuthorizationinfo>();
            customer = ds.CustomerAuthorizationinfo(id.ToString());
            Session["customer"] = customer;
            foreach (var item in customer) {
            model.Branch = item.Branch;
            model.AccountType = item.AccountType;
            model.Customername = item.Customername;
            model.Currency = item.Currency;
            model.Customeraccount = item.Customeraccount;
            model.UserName = item.UserName;
            model.Address = item.Address;
            model.CustomerPhone = item.CustomerPhone;
            model.Email = item.Email;
            model.Profile = item.Profile;
            model.userid = item.userid;
              
            }
            model.authsts = "true";
            model.rjtsts = "false";
            Session["model"] = model;
            return View(model);
        }

        public ActionResult Authorize(int id, String status)
        {

            int response = ds.updatecustomer(id.ToString(), "U");
            if (response != -1)
            {
                Session["result"] = "Customer Authorization Completed Successfuly";
                //return RedirectToAction("Index", "Home", new { area = "" });
                return RedirectToAction("CustomerAuthorization");
            }
            else{

                return RedirectToAction("Details", id);
            }
        }
        public ActionResult Reject(int id, String status)
        {

              int response = ds.updatecustomer(id.ToString(), "R");
              if (response != -1)
              {

                 Session["result"] = "Reject Completed Successfuly";
                 // return RedirectToAction("Index", "Home", new { area = "" });
                      return RedirectToAction("CustomerAuthorization");
              }
              else
              {

                  return RedirectToAction("Details", id);
              }
        }
	}
}