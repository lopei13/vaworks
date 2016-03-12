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
            var shoppingCartItems = db.ShoppingCartItems
                .Include(s => s.Actuator)
                .Include(s => s.Kit)
                .Include(s => s.User)
                .Include(s => s.Valve);
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
        public ActionResult RemoveItem(int shoppingCartItemId, int quantity)
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
