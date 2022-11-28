using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cpanel.Models;
using Cpanel.Context;

namespace Cpanel.Controllers
{
    public class ChqRequestController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /ChqRequest/
        public ActionResult View()
        {
            if (Session["chqmessage"] != null)
            {
                ViewBag.SuccessMessage = Session["chqmessage"].ToString();
                Session["chqmessage"] = null;
            }
            ChqRequest model = new ChqRequest();
            List<ChqRequest> requests = new List<ChqRequest>();
            requests = ds.Chqrequest(Session["user_branch"].ToString());
          /*  if (!customer.Count.Equals(0))
            {
                foreach (var item in customer)
                {
                    model.request_id = item.request_id;
                    model.accountmap = item.accountmap;
                    model.name = item.name;
                    model.booksize = item.booksize;
                    model.date = item.date;

                }
            }*/
            //select c.request_id,branch_name||'-'||curr_name||'-'||act_name||'-'|| SUBSTR(c.account_no,14),c.requested_size,c.req_date,u.user_name from cheque_reqs c,users u,branchs, currency,act_types where req_status='process' and u.user_id=c.user_id and   SUBSTR(c.account_no,3,3)='004' and branchs.branch_code=SUBSTR(c.account_no,3,3) and act_types.act_type_code=SUBSTR(c.account_no,6,5)and currency.curr_code=SUBSTR(c.account_no,11,3) order by c.request_id

            return View(requests);
        }



         
        public ActionResult Accept(int id)
        {
            String message="",sts = "Accpet";
            int response = ds.updatechqsts(id, sts);

         if (!response.Equals(-1))
         {


             message = "Request Accpet Successfully";
             Session["chqmessage"] = message;

         }
         else
         {
             message = "Sorry You Cannot process now, please try later  ";
             Session["chqmessage"] = message;

         }
         return RedirectToAction("View");

        }
      
        public ActionResult Reject(int id)
        {
            String message="", sts = "Reject";
            int response = ds.updatechqsts(id, sts);
            if (!response.Equals(-1))
            {


                message = "Request Reject Successfully";
                Session["chqmessage"] = message;
                
            }
            else
            {
                message = "Sorry You Cannot process now, please try later  ";
                Session["chqmessage"] = message;
                
            }

            // ModelState.AddModelError("", message );
            return RedirectToAction("View");
        }

    }

}