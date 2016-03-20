using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
    public class ConnectionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Search(string email)
        {
            var users = from u in db.Users
                        where u.Email.Contains(email)
                        select u;

            return View(users);
        }

        [HttpPost]
        public ActionResult Connect(string userId)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var contact = db.Users.Find(userId);


            if (user != null && contact != null) {

                if (user.Friend(contact)) {
                    db.SaveChanges();
                    ViewBag.Heading = "Connection made!";
                    ViewBag.Message = $"You are now connected to {contact.Name}";

                    db.SystemMessages.Add(new SystemMessage() {
                        UserId = contact.Id,
                        DateSent = DateTimeOffset.Now,
                        Message = $"You are now connected to {user.Name} from {user.Organization.Name}."
                    });

                    return View("Message");
                } else {
                    ViewBag.Error = "A connection already exists.";
                    return View("Error");
                }
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult RemoveConnection(string userId)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var contact = db.Users.Find(userId);

            if (user != null && contact != null) {

                if (user.Unfriend(contact)) {
                    int results = db.SaveChanges();
                    ViewBag.Heading = "Connection removed";
                    ViewBag.Heading2 = $"{results} connections removed.";
                    ViewBag.Message = $"You are no longer connection to {contact.Name}";
                    return View("Message");
                } else {
                    ViewBag.Error = "A connection already exists.";
                    return View("Error");
                }
            }

            return HttpNotFound();
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
