using Cpanel.Context;
using Cpanel.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cpanel.Controllers
{
    public class AddAccountController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /AddAcount/
        public ActionResult Add()
        {
            if ((Session["cpanelLogin"] == null) || !Session["cpanelLogin"].ToString().Equals("true"))
            {
                return RedirectToAction("Login", "Login");
            }
            
            if (Session["addaccountresult"] != null)
            {
                ViewBag.SuccessMessage = Session["addaccountresult"].ToString();
                Session["addaccountresult"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            model.Branches = ds.PopulateBranchs();
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();
            model.catgories = ds.GetGatgories();
            model.catgories.RemoveAt(0);
            
            return View(model);

        }
        [HttpPost]
        public ActionResult Add(CustomerRegBankinfo model)
        {
            String message;
          //  account model;
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

                if (ModelState.IsValid)
                {
                    String Accountnumber = "99" +model.BranchCode+ model.AccountTypecode+model.CurrencyCode+ model.AccountNumber;
                    Session["usercat"] = model.CategoryCode;
                    Session["Accountold"] = Accountnumber;
                    String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
                    if (response.Equals("This Account is Already exist"))
                    {
                       
                        
                        return RedirectToAction(actionName: "CustomerAccounts", routeValues: new { Account = Accountnumber });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please Check Customer Information Or Customer Status");
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

        public ActionResult CustomerAccounts(String Account)
        {
            try {
                string response = Connecttocore.GetCustaccounts(Account);
                    //"{'1':{'ACT_C_NAME':'¿¿¿¿ ¿¿¿¿¿ ¿¿¿¿ ¿¿¿¿¿¿¿¿','CURRENCY_C_CODE':'001','CUST_I_NO':'467','MOBILE_C_NO':'0','BRANCH_C_CODE':'004','ACT_C_TYPE':'20102'},'tranDateTime':'180118082930','NoOfAct':2,'2':{'ACT_C_NAME':'¿¿¿¿ ¿¿¿¿¿ ¿¿¿¿ ¿¿¿¿¿¿¿¿','CURRENCY_C_CODE':'001','CUST_I_NO':'82','MOBILE_C_NO':'0','BRANCH_C_CODE':'004','ACT_C_TYPE':'20105'},'uuid':'d0088690-368a-4737-a9e0-5f330add73c1','errormsg':'Successfully','errorcode':'1'}";//Connecttocore.GetCustaccounts(Account);
            JObject jobj = new JObject();
            jobj = JObject.Parse(response);
            dynamic result = jobj;
            List<addaccount> items = new List<addaccount>();
            string errormsg = result.errormsg;
            string errorcode = result.errorcode;
            string NoOfAct = result.NoOfAct;
            if (errorcode.Equals("1") && !NoOfAct.Equals("0"))
            {
                int index = Convert.ToInt32(NoOfAct);
                for (int i = 1; i <= index; i++)
                {

                    try
                    {

                        JToken singlerow = result[i.ToString()];
                        JObject newObj = new JObject();
                        dynamic singleObj = singlerow;
                        String Branchname = singleObj.BRANCH_C_CODE;
                        String AccountTypename = singleObj.ACT_C_TYPE;// ds.getaccounttype(singleObj.ACT_C_TYPE);
                        String Currencyname = singleObj.CURRENCY_C_CODE;// ds.getcurrencyname(singleObj.CURRENCY_C_CODE);
                        String accno = singleObj.CUST_I_NO;
                        if(!Account.Substring(13).Equals(accno))
                        {
                        items.Add(new addaccount
                 {
                     AccountID = i + 1,
                     AccountNumber = singleObj.CUST_I_NO,
                     AccountNumbercomplete = "99" + singleObj.BRANCH_C_CODE + singleObj.ACT_C_TYPE + singleObj.CURRENCY_C_CODE + singleObj.CUST_I_NO,
                     Branch = ds.getbranchnameenglish(Branchname),
                     AccountType = ds.getaccounttype(AccountTypename),
                     Currency = ds.getcurrencyname(Currencyname),
                     IsSelected = false,
                 });
                        }
                        //Session["Accountold"] = Account;
                     
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "System Error");

                    }



                }

                accountsresult accountsresult = new accountsresult();
                accountsresult.accountSelected = items;
                return View(accountsresult);
            }
            else
                ModelState.AddModelError("", "System Error");
                }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "System Error");

            }
            return View();
        }
        [HttpPost]
        public ActionResult CustomerAccounts(accountsresult model)
        {
            String result = "",res="";
            List<addaccount> lHob = new List<addaccount>();
            lHob = model.accountSelected;
            foreach (var item in lHob)
            {
                if (item.IsSelected == true)
                {
                    result = ds.addnewacount(Session["Accountold"].ToString(), item.AccountNumbercomplete, Session["usercat"].ToString());
                    String act = item.AccountNumbercomplete.ToString();
                    String s = ds.getbranchnameenglish(act.Substring(2, 3)).ToString() + "-" + ds.getaccounttype(act.Substring(5, 5)).ToString() + "-"
                        + ds.getcurrencyname(act.Substring(10, 3)).ToString() + "-" + act.Substring(13).ToString();

                     res +=" "+ s+" : " + result;
                     
                }
                Session["addaccountresult"] = res;
            }
            return RedirectToAction("Add");
        }


        public ActionResult Authorizer()
        {
            if (Session["authresult"] != null)
            {
                ViewBag.SuccessMessage = Session["authresult"].ToString();
                Session["authresult"] = null;
            }

            //Session["bracode"] = "000";
            String branchcode = Session["bracode"].ToString();

            List<pendingacts> customer = new List<pendingacts>();
            customer = ds.Pendingacounts(branchcode);

            return View(customer);

        }
        public ActionResult Details(int id, String act)
        {
            actAuthorizationinfo model = new actAuthorizationinfo();

            List<actAuthorizationinfo> customer = new List<actAuthorizationinfo>();
            customer = ds.newactAuthorizationinfo(id.ToString(), act);
            Session["customer"] = customer;
            foreach (var item in customer)
            {
                model.Branch = item.Branch;
                model.AccountType = item.AccountType;
                model.Customername = item.Customername;
                model.Currency = item.Currency;
                model.Customeraccount = item.Customeraccount;
                model.completeact = item.completeact;
                model.userid = item.userid;

            }
            model.authsts = "true";
            model.rjtsts = "false";
            Session["model"] = model;
            return View(model);
        }
        public ActionResult Authorize(int id, String act)
        {

            int response = ds.updateAccount(id.ToString(), act, "A");
            if (response != -1)
            {
                Session["authresult"] = "Account Authorization Completed Successfuly";
                //return RedirectToAction("Index", "Home", new { area = "" });
                return RedirectToAction("Authorizer");
            }
            else
            {
                return RedirectToAction(actionName: "Details", routeValues: new { id = id, act = act });

                //return RedirectToAction("Details", id, act);
            }
        }
        public ActionResult Reject(int id, String act)
        {

            int response = ds.updateAccount(id.ToString(), act, "R");
            if (response != -1)
            {

                Session["authresult"] = "Reject Completed Successfuly";
                // return RedirectToAction("Index", "Home", new { area = "" });
                return RedirectToAction("Authorizer");
            }
            else
            {
                return RedirectToAction(actionName: "Details", routeValues: new { id = id, act = act });

                //return RedirectToAction("Details", id, act);
            }
        }
    }


}
