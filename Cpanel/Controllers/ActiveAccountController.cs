﻿
using Cpanel.Context;
using Cpanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpanel.Controllers
{
    public class ActiveAccountController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /ActiveAccount/
        public ActionResult ActiveCustomer()
        {
            if ((Session["cpanelLogin"] == null) || !Session["cpanelLogin"].ToString().Equals("true"))
            {
                return RedirectToAction("Login", "Login");
            }
            
            if (Session["acresult"] != null)
            {
                ViewBag.SuccessMessage = Session["acresult"].ToString();
                Session["acresult"] = null;
            }

            CustomerRegBankinfo model = new CustomerRegBankinfo();
            model.Branches = ds.PopulateBranchs();
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();
            model.catgories = ds.GetGatgories();
            model.catgories.RemoveAt(0);

            Session["regmodel"] = model;
            return View(model);

        }
        [HttpPost]
        public ActionResult ActiveCustomer(CustomerRegBankinfo model)
        {

            string message = "";
            try
            {
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

                    infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, model.UserLog);
                    response = infomodel.lblconfirm;
                    if (response.Equals("This Account is Already exist"))
                    {
                        String act = "99" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
                            model.AccountNumber;
                        Session["Account"] = act;
                        if (!infomodel.status.ToString().Equals("A"))
                        {
                            int result = ds.updatecustomerusingact(act, "A");
                            if (result != -1)
                            {


                                
                                String s = ds.getbranchnameenglish(act.Substring(2, 3)).ToString() + "-" + ds.getaccounttype(act.Substring(5, 5)).ToString() + "-"
                                    + ds.getcurrencyname(act.Substring(10, 3)).ToString() + "-" + act.Substring(13).ToString();

                                Session["acresult"] = "The Customer Account " + s + " Activated Successfully";
                                //return RedirectToAction("Index", "Home", new { area = "" });
                                return RedirectToAction("ActiveCustomer");
                            }
                        }
                        else if (infomodel.status.ToString().Equals("P"))
                        {

                            message = "This Customer Account Is Not Authorized";
                            ModelState.AddModelError("", message);
                            return View(model);
                        }
                        
                        else if (infomodel.status.ToString().Equals("DE"))
                        {
                            message = "This Customer Account Is Deleted";
                            ModelState.AddModelError("", message);
                            return View(model);


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
                            return View(model);
                        }


                    }
                    else
                    {
                        message = "Sorry this account Not Registered ";
                        ModelState.AddModelError("", message);
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

    }
}