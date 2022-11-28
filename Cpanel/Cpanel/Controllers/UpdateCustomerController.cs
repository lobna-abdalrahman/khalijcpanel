using Cpanel.Context;
using Cpanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpanel.Controllers
{
    public class UpdateCustomerController : Controller
    {
        DataSource ds = new DataSource();
     
        //
        // GET: /UpdateCustomer/
        public ActionResult CustInfo()
        {
            if (Session["updatestatus"] != null)
            {
                ViewBag.SuccessMessage = Session["updatestatus"].ToString();
                Session["updatestatus"] = null;
            }
            if (Session["deletestatus"] != null)
            {
                ViewBag.SuccessMessage = Session["deletestatus"].ToString();
                Session["deletestatus"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            model.Branches = ds.PopulateBranchs();
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();

            model.catgories = ds.GetGatgories();
            model.catgories.RemoveAt(0);

            return  View(model);
        }
        [HttpPost]
        public ActionResult CustInfo(CustomerRegBankinfo model)
        {

            string message = "";
            try
            {
                GETpassword model1 = new GETpassword();
                model.Branches = ds.PopulateBranchs();
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
                model.catgories.RemoveAt(0);
                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());

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


                if (ModelState.IsValidField(model.BranchCode) && ModelState.IsValidField(model.AccountNumber) &&
                   ModelState.IsValidField(model.AccountTypecode) && ModelState.IsValidField(model.CurrencyCode))
              
                {
                    custinfo infomodel = new custinfo();

                    String response;

                    infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode);
                    response = infomodel.lblconfirm;
                    if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("A"))
                    {
                        String Accountnumber = "99" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
                            model.AccountNumber;
                        infomodel.Profiles = ds.PopulateProfiles();
                        Session["resultcustinfo"] = infomodel;
                        return RedirectToAction("Detials");

                    }

                    else if (infomodel.status.ToString().Equals("P"))
                    {

                        message = "This Customer Account Is Not Authorized";
                        ModelState.AddModelError("", message);
                        return  View(model);
                    }

                    else if (infomodel.status.ToString().Equals("R"))
                    {
                        message = "This Customer Account Is Rejected";
                        ModelState.AddModelError("", message);
                        return View(model);

                    }

                    else if (infomodel.status.ToString().Equals("A"))
                    {
                        message = "This Customer Account Is  activated already";
                        ModelState.AddModelError("", message);
                        return  View(model);
                    }
                    else if (infomodel.status.ToString().Equals("D"))
                    {
                        message = "This Customer Account Is Deactivated";
                        ModelState.AddModelError("", message);
                        return  View(model);


                    }
                    else if (infomodel.status.ToString().Equals("DE"))
                    {
                        message = "This Customer Account Is Deleted";
                        ModelState.AddModelError("", message);
                        return View(model);


                    }
                    else if (infomodel.status.ToString().Equals("S"))
                    {
                        message = "This Customer Account Is Stoped";
                        ModelState.AddModelError("", message);
                        return  View(model);


                    }
                }


                
            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);

            }
            return  View(model);

        }
        
            public ActionResult Detials()
        {
            custinfo model = new custinfo();

            model = (custinfo)Session["resultcustinfo"];

            return  View(model);
        }
            public ActionResult Update(String userid)
        {
            custinfo model = new custinfo();
            model = (custinfo)Session["resultcustinfo"];
            

            return  View(model);
        }
        [HttpPost]
            public ActionResult Update(custinfo model)
            {

              //  custinfo model = new custinfo();
                String message = "";
             //   model = (custinfo)Session["updatecustinfo"];
                try { 

            model.Profiles = ds.PopulateProfiles();

                var selectedProfile = model.Profiles.Find(p => p.Value == model.profileCode.ToString());
                if (selectedProfile != null)
                {
                    selectedProfile.Selected = true;

                }
           
                int result = ds.Updatecustomer("U", model);
                if (result != -1)
                {
                    Session["updatestatus"] = "These Customer information has been Updated";
                    return RedirectToAction("CustInfo");
                }
                else
                {
                    message = "Problem";
                    ModelState.AddModelError("", message);

                }
                }
                catch (Exception ex)
                {
                    message = "Please Contact for Support";
                    ModelState.AddModelError("", "Something is missing" + message);

                }

                return View(model);
            }
            public ActionResult Delete(String userid)
            {
                custinfo model = new custinfo();
                String message = "";
                model = (custinfo)Session["resultcustinfo"];
                int result = ds.Updatecustomer("DE", model);
                if (result != -1)
                {
                    Session["deletestatus"] = "These Customer Account has been Deleted";
                    return RedirectToAction("CustInfo");
                }
                else
                {
                    message = "Problems";
                    ModelState.AddModelError("", message);

                }


                return View(model);
            }
	}
}