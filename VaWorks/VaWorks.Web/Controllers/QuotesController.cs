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
    public class QuotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quotes
        public ActionResult Index()
        {
            var quotes = db.Quotes.Include(q => q.CreatedBy);
            return View(quotes.ToList());
        }

        public ActionResult QuotesGrid()
        {
            var userId = User.Identity.GetUserId();
            
            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user.Organization != null) {
                var users = user.Organization.GetAllUsers().Select(o => o.Id);
                var contacts = user.Contacts.Select(c => c.Id);

                var quotes = db.Quotes.Where(q => q.CreatedById == userId ||
                (users.Contains(q.CustomerId) && q.IsSent) ||
                users.Contains(q.CreatedById) ||
                contacts.Contains(q.CreatedById) ||
                (q.CustomerId == userId && q.IsSent)).OrderByDescending(q => q.QuoteNumber);

                return PartialView("_QuotesGrid", quotes);
            } else {
                return PartialView("_QuoteGrid", new List<Quote>());
            }

        }

        public ActionResult ViewQuote(int quoteId)
        {
            var quote = db.Quotes.Find(quoteId);
            if (base.Request.UrlReferrer == null || base.Request.UrlReferrer.Host != base.Request.Url.Host) {
                ViewBag.ReturnUrl = new UrlHelper().Action("Index", "Account");
            } else {
                ViewBag.ReturnUrl = base.Request.UrlReferrer;
            }

            if(quote == null) {
                return HttpNotFound();
            }

            if(!quote.IsSent && quote.CreatedById != User.Identity.GetUserId()) {
                return HttpNotFound();
            }

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

        // GET: Quotes/Edit/5
        public ActionResult Edit(int? id)
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
            // populate the organizations
            // set the user
            return View(quote);
        }

        // POST: Quotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Quote quote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quote).State = EntityState.Modified;
                db.SaveChanges();
                return Edit(quote.QuoteId);
            }
            // populate the organizations
            // set the user
            return View(quote);
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
                var dis = org.Discounts.Where(d => d.Quantity < i.Quantity).OrderBy(d => d.Quantity).FirstOrDefault();
                double discount = 1;
                if (dis != null) {
                    discount = dis.DiscountPercentage / 100;
                }

                double listPrice = db.Kits.Where(k => k.KitNumber == i.KitNumber).FirstOrDefault().Price;
                i.Discount = discount;
                i.PriceEach = listPrice * discount;
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
            mailer.Quote(quote, quote.Customer.Email).SendAsync();

            db.SystemMessages.Add(new SystemMessage() {
                UserId = quote.CustomerId,
                Message = $"{quote.SalesPerson} created a quote for you.  Quote: {quote.QuoteNumber}."
            });

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
