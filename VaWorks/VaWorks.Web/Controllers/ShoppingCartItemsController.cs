using EvoPdf;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
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

        public ActionResult CartLink()
        {
            var id = User.Identity.GetUserId();
            var count = db.ShoppingCartItems.Where(i => i.UserId == id).Count();
            return View(count);
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

            
            var quoteNumber = items.FirstOrDefault().QuoteNumber;
            var revision = "";
            if (quoteNumber == null) {
                // get the next quote number
                var e = db.QuoteNumber.OrderByDescending(n => n.Number).FirstOrDefault();
                if (e == null) {
                    e = new QuoteNumber() {
                        Number = 20000
                    };
                    db.QuoteNumber.Add(e);
                }
                e.Number += 1;
                db.SaveChanges();
                quoteNumber = e.Number;
            } else {
                // lets get the next rev
                var d = db.Quotes.Where(q => q.QuoteNumber == quoteNumber).OrderByDescending(q => q.Revision).FirstOrDefault();

                revision = RevManager.Next(d.Revision);

            }
            

            // take the shopping cart and turn it into a quote
            Quote quote = new Quote() {
                CreatedById = userId,
                CreatedDate = DateTimeOffset.Now,
                IsSent = true,
                QuoteNumber = (int)quoteNumber,
                OrganizationId = org.OrganizationId,
                Revision = revision,
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
                    var dis = org.Discounts.Where(d => d.Quantity >= i.Quantity).OrderBy(d => d.Quantity).FirstOrDefault();
                    double discount = 0;
                    if (dis != null) {
                        discount = dis.DiscountPercentage / 100;
                    }

                    QuoteItem item = new QuoteItem() {
                        Actuator = i.Actuator.ToString(),
                        Valve = i.Valve.ToString(),
                        KitNumber = i.Kit.KitNumber,
                        Description = i.ToString(),
                        Discount = discount,
                        Quantity = i.Quantity,
                        PriceEach = i.Kit.Price * (1 - discount),
                        TotalPrice = i.Kit.Price * (1 - discount) * i.Quantity
                    };

                    quote.Total += item.TotalPrice;
                    quote.Items.Add(item);
                }
            }


            // clear cart
            db.ShoppingCartItems.RemoveRange(db.ShoppingCartItems.Where(c => c.UserId == userId));

            db.SaveChanges();

            IUserMailer mailer = new UserMailer();

            // send a message
            string message = $"Thank you for submitting quote number {quoteNumber}.  You should receive an email with the quote and drawings attached.  ";
            if(quote.Items.Any(i => i.PriceEach == 0)) {
                message += "It looks like some of the items you requested have not been priced yet.  A salesperson will review the items and get back to you.";
            }

            var msg = mailer.Quote(quote, quote.Customer.Email);

            HashSet<string> numbers = new HashSet<string>();

            foreach(var item in quote.Items) {
                string image = Server.MapPath($"~/Content/Images/{item.KitNumber}.jpg");
                if (System.IO.File.Exists(image)) {

                    string code = item.KitNumber.Split('-').LastOrDefault();

                    string mat = db.KitMaterials.Where(m => m.Code == code).FirstOrDefault().Name;

                    string url = Url.Action("ViewDrawing", "ShoppingCartItems", new { kitNumber = item.KitNumber, description = mat + " " + item.Description }, Request.Url.Scheme);

                    HtmlToPdfConverter htmlToPdf = new HtmlToPdfConverter();
                    htmlToPdf.TriggeringMode = TriggeringMode.Auto;
                    htmlToPdf.LicenseKey = "jwERABMTABcYABcOEAATEQ4REg4ZGRkZABA=";
                    htmlToPdf.ConversionDelay = 0;
                    htmlToPdf.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;
                    htmlToPdf.PdfDocumentOptions.PdfPageSize = new PdfPageSize(792, 612);
                    htmlToPdf.PdfDocumentOptions.FitHeight = true;

                    byte[] data = htmlToPdf.ConvertUrl(url);

                    //string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), item.KitNumber + ".PDF");
                    //System.IO.FileStream fs = System.IO.File.OpenWrite(tempPath);
                    //fs.Write(data, 0, data.Length);
                    //fs.Close();
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(data, false);
                    msg.Attachments.Add(new System.Net.Mail.Attachment(ms, $"{item.KitNumber}_{item.Actuator}_{item.Valve}_.PDF"));            
                    
                } else {
                    string file = Server.MapPath($"~/Content/Drawings/{item.KitNumber}.pdf");
                    if (System.IO.File.Exists(file)) {
                        msg.Attachments.Add(new System.Net.Mail.Attachment(file));
                    }
                }

                // get the VES doc
                var vesNum = item.KitNumber.Split('-').LastOrDefault();
                if (!string.IsNullOrEmpty(vesNum)) {
                    if (!numbers.Contains(vesNum)) {
                        numbers.Add(vesNum);
                        var doc = db.Documents.Where(d => d.Name.Contains(vesNum)).FirstOrDefault();
                        if (doc != null) {
                            System.IO.MemoryStream ms = new System.IO.MemoryStream(doc.FileData, false);
                            msg.Attachments.Add(new System.Net.Mail.Attachment(ms, doc.FileName));
                        }
                    }
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
                    Message = $"{user.Name} from {user.Organization.Name} submitted quote number {quoteNumber}.  Please review and get in touch with the customer.  Email: {user.Email}, Phone: {user.PhoneNumber}. "
                });

                var msg2 = mailer.QuoteSubmit(quote, sales.Email);
                msg2.SendAsync();
            }

            db.SaveChanges();

            return RedirectToAction("ViewQuote", "Quotes", new { quoteId = quote.QuoteId });
        }

        [AllowAnonymous]
        public ActionResult ViewDrawing(string kitNumber, string description)
        {
            return View("ViewDrawing", new ViewModels.KitDrawingViewModel() {
                KitNumber = kitNumber,
                Description = description
            });
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

    public static class RevManager
    {
        private static List<string> _revisions = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "R", "T", "U", "V", "W", "Y", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AJ", "AK", "AL", "AM", "AN", "AP", "AR", "AT", "AU", "AV", "AW", "AY", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BJ", "BK", "BL", "BM", "BN", "BP", "BR", "BT", "BU", "BV", "BW", "BY", "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CJ", "CK", "CL", "CM", "CN", "CP", "CR", "CT", "CU", "CV", "CW", "CY", "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DJ", "DK", "DL", "DM", "DN", "DP", "DR", "DT", "DU", "DV", "DW", "DY", "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EJ", "EK", "EL", "EM", "EN", "EP", "ER", "ET", "EU", "EV", "EW", "EY", "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FJ", "FK", "FL", "FM", "FN", "FP", "FR", "FT", "FU", "FV", "FW", "FY", "GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GJ", "GK", "GL", "GM", "GN", "GP", "GR", "GT", "GU", "GV", "GW", "GY", "HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HJ", "HK", "HL", "HM", "HN", "HP", "HR", "HT", "HU", "HV", "HW", "HY", "JA", "JB", "JC", "JD", "JE", "JF", "JG", "JH", "JJ", "JK", "JL", "JM", "JN", "JP", "JR", "JT", "JU", "JV", "JW", "JY", "KA", "KB", "KC", "KD", "KE", "KF", "KG", "KH", "KJ", "KK", "KL", "KM", "KN", "KP", "KR", "KT", "KU", "KV", "KW", "KY", "LA", "LB", "LC", "LD", "LE", "LF", "LG", "LH", "LJ", "LK", "LL", "LM", "LN", "LP", "LR", "LT", "LU", "LV", "LW", "LY", "MA", "MB", "MC", "MD", "ME", "MF", "MG", "MH", "MJ", "MK", "ML", "MM", "MN", "MP", "MR", "MT", "MU", "MV", "MW", "MY", "NA", "NB", "NC", "ND", "NE", "NF", "NG", "NH", "NJ", "NK", "NL", "NM", "NN", "NP", "NR", "NT", "NU", "NV", "NW", "NY", "PA", "PB", "PC", "PD", "PE", "PF", "PG", "PH", "PJ", "PK", "PL", "PM", "PN", "PP", "PR", "PT", "PU", "PV", "PW", "PY", "RA", "RB", "RC", "RD", "RE", "RF", "RG", "RH", "RJ", "RK", "RL", "RM", "RN", "RP", "RR", "RT", "RU", "RV", "RW", "RY", "TA", "TB", "TC", "TD", "TE", "TF", "TG", "TH", "TJ", "TK", "TL", "TM", "TN", "TP", "TR", "TT", "TU", "TV", "TW", "TY", "UA", "UB", "UC", "UD", "UE", "UF", "UG", "UH", "UJ", "UK", "UL", "UM", "UN", "UP", "UR", "UT", "UU", "UV", "UW", "UY", "VA", "VB", "VC", "VD", "VE", "VF", "VG", "VH", "VJ", "VK", "VL", "VM", "VN", "VP", "VR", "VT", "VU", "VV", "VW", "VY", "WA", "WB", "WC", "WD", "WE", "WF", "WG", "WH", "WJ", "WK", "WL", "WM", "WN", "WP", "WR", "WT", "WU", "WV", "WW", "WY", "YA", "YB", "YC", "YD", "YE", "YF", "YG", "YH", "YJ", "YK", "YL", "YM", "YN", "YP", "YR", "YT", "YU", "YV", "YW", "YY" };

        public static string Next(string rev)
        {
            if (string.IsNullOrEmpty(rev)) {
                return _revisions[0];
            }

            int index = _revisions.IndexOf(rev) + 1;

            if (index >= _revisions.Count) {
                return "Too many revisions";
            }

            return _revisions[index];
        }
    }
}
