using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cpanel.Context;
using Cpanel.Models;
using Newtonsoft.Json.Linq;

namespace Cpanel.Controllers
{
    public class CustomerRegistrationController : Controller
    {
        DataSource ds = new DataSource();

        //

        // GET: /CustomerRegistration/
        public ActionResult Registration()
        {

            if (Session["message"] != null)
            {
                ViewBag.SuccessMessage = Session["message"].ToString();
                Session["message"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            model.Branches = ds.PopulateBranchs();
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();

            model.catgories = ds.GetGatgories();
            model.catgories.RemoveAt(0);
            model.Channels = ds.Channels();
            Session["regmodel"] = model;
            return View(model);
        }




        [HttpPost]
        public ActionResult Registration(CustomerRegBankinfo model)
        {
            string message = "";
            try
            {
                model.UserLog = "N/A";
                model.Branches = ds.PopulateBranchs();
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
                model.catgories.RemoveAt(0);
                model.Channels = ds.Channels();
                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
               
             // model.Channel = "1";

                if (selectedBranch != null)
                {
                    selectedBranch.Selected = true;

                }
                if (selectedAccType != null)
                {
                    selectedAccType.Selected = true;

                }
                if (selectedCurrency != null)
                {
                    selectedCurrency.Selected = true;

                }
                if (selectedcategory != null)
                {
                    selectedcategory.Selected = true;

                }


                if (ModelState.IsValid)
                {


                    String response = ds.custregcheck(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode);

                    if (response.Equals("This Account is available"))
                    {
                        String act = "99" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
                            model.AccountNumber;
                        Session["Account"] = act;
                        response = Connecttocore.GetCustinfo(act);
                        JObject jobj = new JObject();
                        jobj = JObject.Parse(response);
                        dynamic result = jobj;

                        string responseStatus = result.responseStatus;
                        string responseMessage = result.responseMessage;
                        string bal = result.result;
                        string[] separators = { ",", ":" };
                        string value = bal;
                        string[] acc = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        String custname;
                        String custID;
                        String custphone;

                        if (acc.Length == 6)
                        {
                            custID = acc[1].ToString();
                            custname = acc[3].ToString();

                            custphone = acc[5].ToString();
                            Session["custID"] = custID;
                            Session["custname"] = custname;
                            Session["custphone"] = custphone;
                            Session["custcat"] = model.CategoryCode;
                            if (model.SelectedChannelsID.Length.Equals(model.Channels.Count))
                            {
                                Session["service"] = "3";
                            }
                            else {
                                foreach (var item in model.SelectedChannelsID)
                                {

                                    if (item.Equals("2"))
                                    {  //append each checked records into StringBuilder   
                                        Session["ebranch"] = "T";
                                        Session["service"] = "2";
                                    }
                                    else if (item.Equals("1"))
                                    {  //append each checked records into StringBuilder   
                                        Session["ebank"] = "T";
                                        Session["service"] = "1";
                                    }
                                    else
                                    {
                                        Session["service"] = "0";
                                    }


                                }
                            }  
                            return RedirectToAction("custinfo");
                        }

                        else
                        {
                            message = "Please check customer information something wrong ";
                            ModelState.AddModelError("", message);
                            return View(model);
                        }
                        // 
                        //}
                        //else
                        //{
                        //    message = "Sorry You Cannot register to this account because  ";
                        //    ModelState.AddModelError("", message + response);
                        //    return View(model);
                        //}
                    }
                    else
                    {
                        message = "Sorry You Cannot register to this account because  ";
                        ModelState.AddModelError("", message + response);
                        return View(model);
                    }
                }
                else
                {
                    message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);

                }
            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);

            }
            return View(model);

        }

        public ActionResult custinfo()
        {
            CustomerRegBankinfo2 model = new CustomerRegBankinfo2();
            model.CustomerID = Session["custID"].ToString();
            model.CustomerName = Session["custname"].ToString();
            model.CustomerPhone = Session["custphone"].ToString();
            return View(model);
        }
        [HttpPost]
        public ActionResult custinfo(CustomerRegBankinfo2 model)
        {



            return RedirectToAction("Registrationpersonalinfo");
        }
        public ActionResult Registrationpersonalinfo()
        {
            CustomerRegpersonalinfo model = new CustomerRegpersonalinfo();
            model.Profiles = ds.PopulateProfiles();

            return View(model);
        }
        [HttpPost]
        public ActionResult Registrationpersonalinfo(CustomerRegpersonalinfo model)
        {
            String message;
            model.Profiles = ds.PopulateProfiles();

            var selectedProfile = model.Profiles.Find(p => p.Value == model.profileCode.ToString());
            if (selectedProfile != null)
            {
                selectedProfile.Selected = true;

            }

            if (ModelState.IsValid)
            {
                String username = model.UserName;
                String email = model.Email;
                String address = model.Address;
                String account = Session["Account"].ToString();
                String customerprofile = model.profileCode.ToString();
                String CustomerID = Session["custID"].ToString();
                String CustomerName = Session["custname"].ToString();
                String CustomerPhone = Session["custphone"].ToString();
                String customercatgory = Session["custcat"].ToString();
                String CUSTOMERSERVICE= Session["service"].ToString();
 
                int response = ds.custreg(CustomerID, CustomerName, account, username, address, CustomerPhone, email, customerprofile, customercatgory, CUSTOMERSERVICE);

                if (!response.Equals(-1))
                {


                    message = "Customer Registration Complete Successfully";
                    Session["message"] = message;
                    // ModelState.AddModelError("", message );
                    return RedirectToAction("Registration");
                }
                else
                {
                    message = "Sorry You Cannot register to this account now, please try later  ";
                    ModelState.AddModelError("", message);
                    return View(model);
                }
            }
            else
            {
                message = "All Fields are required ";
                ModelState.AddModelError("", "Something is missing" + message);
                return View(model);
            }

        }
    }
}