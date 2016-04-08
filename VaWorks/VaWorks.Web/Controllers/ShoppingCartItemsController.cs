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
using VaWorks.Web.Mailers;

namespace VaWorks.Web.Controllers
{
    [Authorize]
    public class ShoppingCartItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult ShoppingCartItemCount()
        {
            var id = User.Identity.GetUserId();
            var count = db.ShoppingCartItems.Where(i => i.UserId == id).Count();

            return PartialView("BadgeCount", count > 0 ? count.ToString() : "");
        }

        // GET: ShoppingCartItems
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var shoppingCartItems = db.ShoppingCartItems
                .Include(s => s.Actuator)
                .Include(s => s.Kit)
                .Include(s => s.User)
                .Include(s => s.Valve).Where(u => u.UserId == userId);
            return View(shoppingCartItems.ToList());
        }

        // POST: ShoppingCartItems/UpdateQuantity
        [HttpPost]
        public ActionResult UpdateQuantity(int shoppingCartItemId, int quantity)
        {
            var item = db.ShoppingCartItems.Where(i => i.ShoppingCartItemId == shoppingCartItemId).FirstOrDefault();
            if(item != null) {
                item.Quantity = quantity;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: ShoppingCartItems/RemoveItem
        [HttpPost]
        public ActionResult RemoveItem(int shoppingCartItemId)
        {
            var item = db.ShoppingCartItems.Where(i => i.ShoppingCartItemId == shoppingCartItemId).FirstOrDefault();
            if (item != null) {
                db.ShoppingCartItems.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: ShoppingCartItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCartItems shoppingCartItems = db.ShoppingCartItems.Find(id);
            if (shoppingCartItems == null)
            {
                return HttpNotFound();
            }
            return View(shoppingCartItems);
        }

        public ActionResult SubmitQuote(string title = null)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var org = db.Organizations.Where(o => o.OrganizationId == user.OrganizationId).Include(o => o.Discounts).FirstOrDefault();
            var items = db.ShoppingCartItems.Where(c => c.UserId == userId)
                .Include(i => i.Actuator)
                .Include(i => i.Valve)
                .Include(i => i.Kit);

            if (items.Count() == 0) {
                ViewBag.Error = "There are no items in your shopping cart.  Please add some items.";
                return View("Error");
            }

            var sales = user.Contacts.Where(c => c.IsSales).FirstOrDefault();

            if(sales == null) {
                sales = db.Users.Where(u => u.IsSales).FirstOrDefault();
            }

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

            // take the shopping cart and turn it into a quote
            Quote quote = new Quote() {
                CreatedById = userId,
                CreatedDate = DateTimeOffset.Now,
                IsSent = true,
                QuoteNumber = quoteNumber.Number,
                CustomerName = user.Name,
                CreatedByName = user.Name,
                CustomerId = user.Id,
                CompanyName = user.Organization.Name,
                Address1 = user.Organization.Address1,
                Address2 = user.Organization.Address2,
                City = user.Organization.City,
                Country = user.Organization.Country,
                State = user.Organization.State,
                PostalCode = user.Organization.PostalCode,
                SalesPerson = sales != null ? sales.Name : "VanAire",
                Title = title,
                IsOrder = false
            };

            db.Quotes.Add(quote);

            foreach (var i in items) {
                if (i.Actuator != null && i.Valve != null && i.Kit != null) {
                    // get the discount
                    var dis = org.Discounts.Where(d => d.Quantity < i.Quantity).OrderBy(d => d.Quantity).FirstOrDefault();
                    double discount = 1;
                    if (dis != null) {
                        discount = dis.DiscountPercentage / 100;
                    }

                    quote.Items.Add(new QuoteItem() {
                        Actuator = i.Actuator.ToString(),
                        Valve = i.Valve.ToString(),
                        KitNumber = i.Kit.KitNumber,
                        Description = i.ToString(),
                        Discount = discount,
                        Quantity = i.Quantity,
                        PriceEach = i.Kit.Price * discount,
                        TotalPrice = i.Kit.Price * discount * i.Quantity
                    });
                }
            }

            // clear cart
            db.ShoppingCartItems.RemoveRange(db.ShoppingCartItems.Where(c => c.UserId == userId));

            IUserMailer mailer = new UserMailer();

            // send a message
            string message = $"Thank you for submitting quote number {quoteNumber.Number}.  You should receive an email with the quote and drawings attached.  ";
            if(quote.Items.Any(i => i.PriceEach == 0)) {
                message += "It looks like some of the items you requested have not been priced yet.  A salesperson will review the items and get back to you.";
            }

            var msg = mailer.Quote(quote, quote.Customer.Email);

            foreach(var item in quote.Items) {
                string file = Server.MapPath($"~/Content/Drawings/{item.KitNumber}.pdf");
                if (System.IO.File.Exists(file)) {
                    msg.Attachments.Add(new System.Net.Mail.Attachment(file));
                }
            }

            msg.SendAsync();

            db.SystemMessages.Add(new SystemMessage() {
                UserId = userId,
                DateSent = DateTimeOffset.Now,
                Message = message
            });

            

            if (sales != null) {
                db.SystemMessages.Add(new SystemMessage() {
                    UserId = sales.Id,
                    DateSent = DateTimeOffset.Now,
                    Message = $"{user.Name} from {user.Organization.Name} submitted quote number {quoteNumber.Number}.  Please review and get in touch with the customer.  Email: {user.Email}, Phone: {user.PhoneNumber}. "
                });

                var msg2 = mailer.QuoteSubmit(quote, sales.Email);
                msg2.SendAsync();
            }

            db.SaveChanges();

            return RedirectToAction("ViewQuote", "Quotes", new { quoteId = quote.QuoteId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
