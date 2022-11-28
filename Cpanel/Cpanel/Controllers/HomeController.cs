using Cpanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetBankCpanel.Controllers
{
    public class HomeController : Controller
    {
        DataSource ds = new DataSource();
        public ActionResult Index()
        {
            string act = "9900320101001549";
            String s = ds.getbranchnameenglish(act.Substring(2, 3)).ToString() + "-" + ds.getaccounttype(act.Substring(5, 5)).ToString()+"-"
                +ds.getcurrencyname(act.Substring(10,3)).ToString()+"-" + act.Substring(13).ToString();


            if ((Session["cpanelLogin"] == null) || !Session["cpanelLogin"].ToString().Equals("true"))
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["Homemessage"] != null)
            {
                ViewBag.SuccessMessage = Session["Homemessage"].ToString();
                Session["Homemessage"] = null;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        public ActionResult Logout()
        {
            Session["cpanelLogin"] = "0";
            Session["UserId"] = null; 
            Session["user_roleid"] = null;
         Session.Clear();
Session.RemoveAll();
Session.Abandon();

            return RedirectToAction("Login", "Login");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult test2()
        {
            if (Session["Homemessage"] != null)
            {
                ViewBag.SuccessMessage = Session["Homemessage"].ToString();
                Session["Homemessage"] = null;
            }

            //if (Session["Homemessage"] != null)
            //{
            //    ViewBag.SuccessMessage = Session["Homemessage"].ToString();
            //    Session["Homemessage"] = null;
            //}
            return View();
        }
        public ActionResult Test()
        {
            ViewBag.Message = "Your Test page.";

            return View();
        }
    }
    
}
