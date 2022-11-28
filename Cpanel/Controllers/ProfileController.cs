using Cpanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cpanel.Models;

namespace Cpanel.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        DataSource ds = new DataSource();
        //
        // GET: /AddAcount/
        public ActionResult viewProfile()
        {

            Profilemangement model = new Profilemangement();

            model.catgories = ds.GetGatgories();
            model.profiles = ds.GetProfiles();
            return View(model);
        }
        public ActionResult NewProfile()
        {
            if (Session["addprofileresult"] != null)
            {
                ViewBag.SuccessMessage = Session["addprofileresult"].ToString();
                Session["addprofileresult"] = null;
                
            }
            Session["profilelist"] = null;
            Session["menu_category"] = null;

            Profilemangement model = new Profilemangement();

            model.catgories = ds.GetGatgories();
            return View(model);

        }

        [HttpPost]
        public ActionResult NewProfile(Profilemangement model)
        {  
            model.catgories = ds.GetGatgories();
            var selectedcategory = model.catgories.Find(p => p.Value == model.menu_category.ToString());
            if (selectedcategory != null)
            {
                selectedcategory.Selected = true;

            }
            if (model.menu_category != null)
            {
                Session["profilelist"] = true;
                Session["menu_category"] = model.menu_category;
                List<pageparameter> items = new List<pageparameter>();
                items = ds.PopulateProfilemangement(model.menu_category);

                model.pages = items;
                return View(model);
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult Addprofile(Profilemangement model)
        {

            String result = "", res = "";
            String message;
            try
            {
                model.catgories = ds.GetGatgories();
                if ( ModelState.IsValidField(model.profilename))
                {

                    List<pageparameter> lHob = new List<pageparameter>();
                    lHob = model.pages;
                    foreach (var item in lHob)
                    {
                        if (item.IsSelected == true)
                        {
                            result = ds.addnewprofile(model.profilename, item.menuid, item.menuparentid);
                            res += " " + item.menuname + " : " + result;
                            Session["addprofileresult"] = "Profile Creation  Complete Successfully";
                        }

                    }

                    return RedirectToAction("NewProfile");
                  
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