using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VaWorks.Web.Data;
using VaWorks.Web.Data.Entities;
using VaWorks.Web.Extensions;
using VaWorks.Web.Mailers;

namespace VaWorks.Web.Controllers
{
    [Authorize]
    public class QuotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quotes
        public ActionResult Index()
        {
            var quotes = db.Quotes.Include(q => q.CreatedBy).OrderByDescending(q => q.CreatedDate);
            return View(quotes);
        }

        public ActionResult CompanyQuotes()
        {
            var userId = User.Identity.GetUserId();

            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user.Organization != null) {
                var orgs = user.Organization.GetAllOrganizations().Select(o => o.OrganizationId);
                var contacts = user.Contacts.Select(c => c.Id);

                var quotes = db.Quotes.Where(q => orgs.Contains(q.OrganizationId)).OrderByDescending(q => q.CreatedDate);

                return View("QuotesGrid", quotes);
            } else {
                return View("QuotesGrid", new List<Quote>());
            }
        }

        public ActionResult MyQuotes()
        {
            var userId = User.Identity.GetUserId();
            var quotes = db.Quotes.Where(q => q.CreatedById == userId).OrderByDescending(q => q.CreatedDate);
            return View("QuotesGrid", quotes);
        }

        [AllowAnonymous]
        public ActionResult ViewQuote(int quoteId)
        {
            var quote = db.Quotes.Find(quoteId);

            ViewBag.ReturnUrl = new UrlHelper(HttpContext.Request.RequestContext).Action("MyQuotes", "Quotes");
 

            if(quote == null) {
                return HttpNotFound();
            }

            //if(!quote.IsSent && quote.CreatedById != User.Identity.GetUserId()) {
            //    return HttpNotFound();
            //}

            return View(quote);
        }

        // GET: Quotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            return View(quote);
        }

        // GET: Quotes/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

           
            Quote quote = new Quote() {
                SalesPerson = user.Name
            };

            PopulateDropDown();

            return View();
        }

        // POST: Quotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string customerId = null, string title = null)
        {
            if(string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(title)) {
                ModelState.AddModelError("", new Exception("Select a user and enter a title."));
                PopulateDropDown(customerId);
                return View();
            }

            var customer = db.Users.Find(customerId);
            var org = customer.Organization;

            var salesId = User.Identity.GetUserId();
            var sales = db.Users.Find(salesId);


            // get the next quote number
            var quoteNumber = db.QuoteNumber.OrderByDescending(n => n.Number).FirstOrDefault();
            if (quoteNumber == null) {
                quoteNumber = new QuoteNumber() {
                    Number = 20000
                };
                db.QuoteNumber.Add(quoteNumber);
            }
            quoteNumber.Number += 1;

            db.SaveChanges();

            Quote quote = new Quote() {
                CreatedById = salesId,
                CreatedDate = DateTimeOffset.Now,
                CreatedByName = sales.Name,
                IsSent = false,
                QuoteNumber = quoteNumber.Number,
                CustomerName = customer.Name,
                CustomerId = customer.Id,
                CompanyName = customer.Organization.Name,
                Address1 = customer.Organization.Address1,
                Address2 = customer.Organization.Address2,
                City = customer.Organization.City,
                Country = customer.Organization.Country,
                State = customer.Organization.State,
                PostalCode = customer.Organization.PostalCode,
                SalesPerson = sales != null ? sales.Name : "VanAire",
                Title = title,
                IsOrder = false
            };

            db.Quotes.Add(quote);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = quote.QuoteId });
        }

        public ActionResult EditTitle(int? id, string returnUrl)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Quote quote = db.Quotes.Find(id);

            if (quote == null) {
                return HttpNotFound();
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(quote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTitle(int? quoteId, string title, string returnUrl)
        {
            if(quoteId == null || string.IsNullOrEmpty(title)) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Quote quote = db.Quotes.Find(quoteId);

            if(quote == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            quote.Title = title;

            db.SaveChanges();

            return Redirect(returnUrl);
        }

        // GET: Quotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.Quotes.Include("Items").Where(q=> q.QuoteId == id).FirstOrDefault();
            if (quote == null)
            {
                return HttpNotFound();
            }
            // populate the organizations
            // set the user
            return View(quote);
        }

        // POST: Quotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm(int quoteId)
        {
            var userId = User.Identity.GetUserId();
            
            var quote = db.Quotes.Include("Items").Where(q => q.QuoteId == quoteId).FirstOrDefault();

            if(quote != null) {
                // add all of these items to the users current shopping cart
                db.ShoppingCartItems.RemoveRange(db.ShoppingCartItems.Where(i => i.UserId == userId));

                foreach(var i in quote.Items) {
                    var kit = db.Kits.Where(k => k.KitNumber == i.KitNumber).FirstOrDefault();
                    var valve = db.Valves.Where(v => v.Manufacturer + " " + v.Model + " " + v.Size == i.Valve).FirstOrDefault();
                    var actuator = db.Actuators.Where(a => a.Manufacturer + " " + a.Model + " " + a.Size == i.Actuator).FirstOrDefault();

                    db.ShoppingCartItems.Add(new ShoppingCartItems() {
                        UserId = userId,
                        Actuator = actuator,
                        Valve = valve,
                        Kit = kit,
                        QuoteNumber = quote.QuoteNumber,
                        Quantity = i.Quantity
                    });
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index", "ShoppingCartItems", null);
        }

        public ActionResult AddItem(int quoteId)
        {
            var quote = db.Quotes.Find(quoteId);
            return View(quote);
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int quoteItemId, int quantity)
        {
            var quoteItem = db.QuoteItems.Find(quoteItemId);
            quoteItem.Quantity = quantity;

            // we need to update the total on the quote
            var quote = db.Quotes.Where(q => q.QuoteId == quoteItem.QuoteId).FirstOrDefault();
            var customer = db.Users.Find(quote.CustomerId);
            var org = db.Organizations.Where(o => o.OrganizationId == customer.OrganizationId).FirstOrDefault();
            quote.Total = 0;
            foreach (var i in quote.Items) {
                var dis = org.Discounts.Where(d => d.Quantity >= i.Quantity).OrderBy(d => d.Quantity).FirstOrDefault();
                double discount = 0;
                if (dis != null) {
                    discount = dis.DiscountPercentage / 100;
                }

                double listPrice = db.Kits.Where(k => k.KitNumber == i.KitNumber).FirstOrDefault().Price;
                i.Discount = discount;
                i.PriceEach = listPrice * (1 - discount);
                i.TotalPrice = i.PriceEach * i.Quantity;
                quote.Total += i.TotalPrice;
            }

            db.SaveChanges();

            return RedirectToAction("Edit", new { id = quoteItem.QuoteId });
        }

        public ActionResult SendQuote(int quoteId)
        {
            var quote = db.Quotes.Find(quoteId);
            quote.IsSent = true;


            IUserMailer mailer = new UserMailer();
            try {
                mailer.Quote(quote, quote.Customer.Email).Send();
            } catch (Exception ex) {
                return View("MailDown", ex);
            }

            db.SystemMessages.Add(new SystemMessage() {
                UserId = quote.CustomerId,
                Message = $"{quote.SalesPerson} created a quote for you.  Quote: {quote.QuoteNumber}."
            });

            var msg = mailer.Quote(quote, quote.Customer.Email);

            foreach (var item in quote.Items) {
                string image = Server.MapPath($"~/Content/Images/{item.KitNumber}.png");
                if (System.IO.File.Exists(image)) {
                    string url = Url.Action("ViewDrawing", "KitSelection", new { kitNumber = item.KitNumber, description = item.Description }, Request.Url.Scheme);
                    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                    converter.Options.PdfPageSize = SelectPdf.PdfPageSize.Letter;
                    converter.Options.PdfPageOrientation = SelectPdf.PdfPageOrientation.Landscape;
                    converter.Options.WebPageHeight = 6120;
                    converter.Options.WebPageWidth = 7920;
                    SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    doc.Save(ms);
                    msg.Attachments.Add(new System.Net.Mail.Attachment(ms, item.KitNumber + ".PDF", "application/pdf"));
                } else {
                    string file = Server.MapPath($"~/Content/Drawings/{item.KitNumber}.pdf");
                    if (System.IO.File.Exists(file)) {
                        msg.Attachments.Add(new System.Net.Mail.Attachment(file));
                    }
                }

                // get the VES doc
                var vesNum = item.KitNumber.Split('-').LastOrDefault();
                if (!string.IsNullOrEmpty(vesNum)) {
                    var doc = db.Documents.Where(d => d.Name.Contains(vesNum)).FirstOrDefault();
                    if(doc != null) {
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(doc.FileData, false);
                        msg.Attachments.Add(new System.Net.Mail.Attachment(ms, doc.FileName));
                    }
                }

            }

            msg.SendAsync();

            db.SaveChanges();
            return View(quote);
        }

        // GET: Quotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quote quote = db.Quotes.Find(id);
            if (quote == null)
            {
                return HttpNotFound();
            }
            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quote quote = db.Quotes.Find(id);
            db.Quotes.Remove(quote);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateQuoteLink()
        {
            var userId = User.Identity.GetUserId();
            var items = db.ShoppingCartItems.Where(u => u.UserId == userId);

            return View("CreateQuoteLink", items.Count() > 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers

        private void PopulateDropDown(object selected = null)
        {
            var users = from u in db.Users
                        where u.IsSales == false
                        orderby u.Name
                        select u;

            ViewBag.CustomerId = new SelectList(users, "Id", "Name", selected);
        }

        #endregion
    }
}
