using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cpanel.Context;
using Cpanel.Models;
using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Utilities;

namespace Cpanel.Controllers
{
    public class CustomerTransferReportController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /CustomerTransferReport/
        public ActionResult TransferReport()
        {
            CustomerTransferReportViewModel model = new CustomerTransferReportViewModel();
            model.Branches = ds.PopulateBranchs();
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();



            return View(model);
        }

        [HttpPost]
        public ActionResult TransferReport(CustomerTransferReportViewModel model)
        {

            string message = "";

            try
            {
                model.Branches = ds.PopulateBranchs();
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();

                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());

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

                if (ModelState.IsValid)
                {
                    String CustFullAccount = "99" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
                                             model.AccountNumber;

                    string CustID = ds.getCustIDFromAcc(CustFullAccount);

                    if (CustID.IsNullOrWhiteSpace()||CustID.Equals("0"))
                    {
                        message = "This Customer is Un-Available Please make sure to write the Right Account Number ";
                        ModelState.AddModelError("", message);
                        return View(model);
                    }

                    List<CustomerTransferReportViewModel> accass = new List<CustomerTransferReportViewModel>();
                    accass = ds.GetTransferReport(CustID);
                    if (accass.Count > 0)
                    {
                        Session["TransferReport"] = accass;



                        return RedirectToAction("ViewTransferReport");
                    }
                    else
                    {
                        message = "No Transaction tor this account ";
                        ModelState.AddModelError("", message);
                        return View(model);
                    }
                }
            }
            catch (Exception e)
            {
               // Console.WriteLine(e);
                message = "Contact Your support";
                ModelState.AddModelError("", message);
                return View(model);
            }

            return View(model);
        }

        public ActionResult ViewTransferReport()
        {
            List<CustomerTransferReportViewModel> accass = new List<CustomerTransferReportViewModel>();

            accass = (List<CustomerTransferReportViewModel>) Session["TransferReport"];

            
           /* foreach (var item in accass)
            {

                if (item.TranFullReq.Contains(','))
                {
                    
                }
            }
*/

            return View(accass);
        }
    }
}