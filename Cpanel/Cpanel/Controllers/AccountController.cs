using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc; 
using Cpanel.Models;
using Cpanel.Repository;

namespace Cpanel.Controllers
{
    public class AccountController : Controller
    {
        public virtual PartialViewResult Menu()
        {
            IEnumerable<Menu> Menu = null;

            if (Session["cpanel_Menu"] != null)
            {
                Menu = (IEnumerable<Menu>)Session["cpanel_Menu"];
            }
            else
            {
                Menu = MenuData.GetMenus(Session["UserId"].ToString(), Session["user_roleid"].ToString());// pass employee id here
                Session["cpanel_Menu"] = Menu;
            }
            return PartialView("_Menu", Menu);
        }
    }
}