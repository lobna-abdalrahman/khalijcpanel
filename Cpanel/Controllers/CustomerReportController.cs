using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cpanel.Context;
using Cpanel.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace Cpanel.Controllers
{
    public class CustomerReportController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /CustomerReport/
        public ActionResult CustomersReport()
        {

            Custreport model = new Custreport();
            model.Branches = ds.PopulateBranchs();
            model.catgories = ds.GetGatgories();
            model.CustomerStatus = ds.PopulateCustStatus();

            Session["CustReport"] = model;

            return View(model);
        }

        [HttpPost]
        public ActionResult CustomersReport(Custreport model)
        {
            string message = "";

            try
            {

                model.Branches = ds.PopulateBranchs();
                model.catgories = ds.GetGatgories();
                model.CustomerStatus = ds.PopulateCustStatus();

                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedCategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
                var selectedStatus = model.CustomerStatus.Find(p => p.Value == model.StatusCode.ToString());

                if (selectedBranch != null)
                {
                    selectedBranch.Selected = true;

                }
                if (selectedCategory != null)
                {
                    selectedCategory.Selected = true;

                }
                if (selectedStatus != null)
                {
                    selectedStatus.Selected = true;

                }


                if (ModelState.IsValid)
                {
                    List<Custreport> accass = new List<Custreport>();

                    accass = ds.GetBranchUsers(model.BranchCode, model.CategoryCode, model.StatusCode);
                    if (accass.Count > 0)
                    {
                        if (model.BranchCode != "0")
                            Session["Branchname"] = ds.getbranchnameenglish(model.BranchCode) ;
                        else
                            Session["Branchname"] = "All Branches";
                        Session["BranchUsers"] = accass;
                        return RedirectToAction("ViewReport");
                    }


                    else
                    {
                        message = "No Customer Registered";
                        ModelState.AddModelError("",  message);
                        return View(model);
                    }
                }
                else
                {
                    message = "Please Contact us for Support";
                    ModelState.AddModelError("", "Something is missing" + message);
                    return View(model);
                }

            }
            catch (Exception e)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                return View(model);
            }

            return View(model);
        }

        public ActionResult ViewReport()
        {
            List<Custreport> accass = new List<Custreport>();
            accass = (List<Custreport>) Session["BranchUsers"];
            ViewBag.Total = accass.Count;
            ViewBag.Date = DateTime.Now.ToString("dd-MMM-yyyy");
          //  ViewBag.Username = DateTime.Now.ToString("HH:mm:ss");
            ViewBag.Time = DateTime.Now.ToString("HH:mm:ss");
            ViewBag.Branchname = Session["Branchname"].ToString();
            Session["totalcustomer"] = accass.Count;
            return View(accass);
        }


        public FileResult SavePDF()
        {

            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("CustomerReport For "+  Session["Branchname"].ToString()+ dTime.ToString("ddMMMyyyyHHmmss") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 6 columns  
            /*PdfPTable tableLayout = new PdfPTable(5);*/
            PdfPTable tableLayout = new PdfPTable(3);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   

            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        //Add Content
        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {



            PdfPTableHeader tableHeader = new PdfPTableHeader();

                      string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);
            float[] headers = { 50, 20, 50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
           
            //Add Title to the PDF file at the top  

            //List < Employee > UserLog = _context.UserLog.ToList < Employee > ();  
            List<Custreport> UserLog = new List<Custreport>();
            UserLog = (List<Custreport>)Session["BranchUsers"];

            DateTime dTime = DateTime.Now;
            //string UserName = Session["name"].ToString();
            //string AccNo = Session["AccNo"].ToString();
            //string fromDate = Session["fromDate"].ToString();
            //string toDate = Session["toDate"].ToString();
            //string AccountNumber = AccNo.Substring(13);
            //string AccountType = data.getaccounttype(AccNo.ToString().Substring(5, 5));
            //string BranchName = data.getbranchnameenglish(AccNo.ToString().Substring(2, 3));
            //string currency = data.GetCurrencyName(AccNo.Substring(10, 3));
            //String oDate = DateTime.ParseExact(fromDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy");

            //paragraphs
            Paragraph Title = new Paragraph("alkhaleejbank Internet Banking  " ,
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Customer Report For " + Session["Branchname"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));

            /*Paragraph From = new Paragraph("Statement of Account From  : " + DateTime.ParseExact(fromDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy") + " To " + DateTime.ParseExact(toDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/
            Chunk c = new Chunk("Total of Customers Registered : " + Session["totalcustomer"].ToString(),
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
           
            Paragraph Total = new Paragraph(c) ; 

            /*Paragraph AccountNo = new Paragraph("Account No : " + AccountNumber,
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

            /* Paragraph Currency = new Paragraph("Currency : " + currency,
                 new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

            /*Paragraph customerName = new Paragraph("User Name:" + UserName,
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

            //Adding Cells
            Paragraph empty = new Paragraph( "\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor =new BaseColor(0, 192, 239),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(0, 192, 239),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 1,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(0, 192, 239),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 2,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(0, 192, 239),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            /* tableLayout.AddCell(new PdfPCell(new Phrase(accountType))
             {
                 Colspan = 4,
                 PaddingLeft = 10,
                 Rowspan = 1,
                 Border = 0,
                 PaddingBottom = 5,
                 HorizontalAlignment = Element.ALIGN_LEFT
             });

             tableLayout.AddCell(new PdfPCell(new Phrase(AccountNo))
             {
                 Colspan = 1,
                 PaddingRight = 10,
                 Rowspan = 1,
                 Border = 0,
                 PaddingBottom = 5,
                 HorizontalAlignment = Element.ALIGN_RIGHT
             });

             tableLayout.AddCell(new PdfPCell(new Phrase(customerName))
             {
                 Colspan = 4,
                 PaddingLeft = 10,
                 Rowspan = 1,
                 Border = 0,
                 PaddingBottom = 5,
                 HorizontalAlignment = Element.ALIGN_LEFT
             });

             tableLayout.AddCell(new PdfPCell(new Phrase(Currency))
             {
                 Colspan = 1,
                 PaddingRight = 10,
                 Rowspan = 1,
                 Border = 0,
                 PaddingBottom = 5,
                 HorizontalAlignment = Element.ALIGN_RIGHT
             });*/

            ////Add header 

            AddCellToHeader(tableLayout, "Name");
            AddCellToHeader(tableLayout, "Status");
            AddCellToHeader(tableLayout, "Account");
             //AddCellToHeader(tableLayout, "Statement ID");  

            ////Add body  

            foreach (var emp in UserLog)
            {

                AddCellToBody(tableLayout, emp.CustomerName.ToString());
                AddCellToBody(tableLayout, emp.CustStatus.ToString());
                AddCellToBody(tableLayout, emp.AccountNumber.ToString());
               
                //AddCellToBody(tableLayout, emp.StateID.ToString());  

            }
            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                PaddingTop = 20,
               
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT

            });
            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan =3,
                Border = 1,
                PaddingTop = 5,
             BackgroundColor=new BaseColor(0, 192, 239),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        //Header Cells:
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1,new BaseColor(0, 192, 239))))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
               Border = Rectangle.BOX,
      BorderWidth = 1,
      BorderWidthLeft=0,
      BorderWidthRight=0,
      BorderWidthTop=0,

                BackgroundColor = new iTextSharp.text.BaseColor(255, 255,255)
            });
        }


        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);

            const string regex_match_arabic_hebrew = @"[\u0600-\u06FF\u0590-\u05FF]+";
            if (Regex.IsMatch(cellText, regex_match_arabic_hebrew, RegexOptions.IgnoreCase))
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
            else
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                     
                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
        }

    
    
    
    }
}