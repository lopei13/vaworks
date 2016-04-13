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
using VaWorks.Web.ViewModels;

namespace VaWorks.Web.Controllers
{
    [Authorize(Roles = "System Administrator")]
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        List<MessageViewModel> messages = new List<MessageViewModel>();

        // GET: Invitations
        public ActionResult Index()
        {
            return View(db.Invitations.ToList());
        }

        // GET: Invitations/PendingInvitationsCount
        public ActionResult PendingInvitationsCount()
        {
            int count = db.Invitations.Where(i => !i.IsClaimed).Count();

            return PartialView("BadgeCount", count > 0 ? count.ToString() : "");
        }

        // GET: Invitations/PendingRequestsCount
        public ActionResult PendingRequestsCount()
        {
            int count = db.InvitationRequests.Where(r => r.Status == RequestStatus.New).Count();

            return PartialView("BadgeCount", count > 0 ? count.ToString() : "");
        }

        // GET: Invitations/InvitationRequestsList
        public ActionResult InvitationRequestsList()
        {
            var requests = db.InvitationRequests.OrderBy(r => r.Status);

            return View(requests);
        }

        // GET: Invitations/InvitationsList
        public ActionResult InvitationsList()
        {
            var invites = db.Invitations.OrderByDescending(r => r.InvitationId);

            return View(invites);
        }

        [HttpPost]
        public ActionResult DeleteInvitation(int id)
        {
            var invite = db.Invitations.Find(id);
            if(invite != null) {
                db.Invitations.Remove(invite);
                db.SaveChanges();
                AddSuccess("Invitation deleted.");
            } else {
                AddDanger("Invitation not found.");
            }
            return Confirmation();
        }

        [HttpPost]
        public ActionResult ResendInvitation(int id)
        {
            var invite = db.Invitations.Find(id);
            if(invite != null) {
                IUserMailer UserMailer = new UserMailer();
                UserMailer.Invitation(invite).SendAsync();
                AddSuccess("Invitation resent.");
            } else {
                AddDanger("Invitation not found.");
            }
            return Confirmation();
        }

        public ActionResult AcceptRequest(int id)
        {
            var request = db.InvitationRequests.Find(id);
            if(request != null) {
                PopulateDropDown();
                return View("Create", new InvitationViewModel() {
                    Emails = request.Email,
                    RequestId = request.InvitationRequestId
                });
            } else {
                AddWarning("Request not found");
                return Confirmation();
            }
        }

        // GET: Invitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // GET: Invitations/Create
        public ActionResult Create(int? organizationId)
        {
            PopulateDropDown(organizationId);
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvitationViewModel invitation)
        {
            if (ModelState.IsValid) {

                var org = db.Organizations.Find(invitation.OrganizationId);
                List<Invitation> invites = new List<Invitation>();
                foreach (var email in invitation.Emails.Split('\n')) {
                    Invitation invite = new Invitation() {
                        CreatedDate = DateTimeOffset.Now,
                        Email = email.Trim(),
                        SalesPersonEmail = invitation.SalesPersonEmail,
                        Company = org.Name,
                        OrganizationId = invitation.OrganizationId,
                        Type = invitation.Type
                    };

                    invites.Add(invite);
                    db.Invitations.Add(invite);
                }

                int count = db.SaveChanges();
                AddSuccess($"{count} invitations created.");

                // set the invite request to granted
                var request = db.InvitationRequests.Find(invitation.RequestId);
                if(request != null) {
                    request.Status = RequestStatus.Granted;
                    db.SaveChanges();
                }

                if (count > 0) {
                    IUserMailer UserMailer = new UserMailer();
                    foreach (var i in invites) {
                        UserMailer.Invitation(i).SendAsync();
                    }
                }
                return Confirmation();
            }

            PopulateDropDown(invitation.OrganizationId);
            return View(invitation);
        }

        // GET: Invitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InvitationId,OrganizationId,InvitationCode,Email,Type,CreatedDate,SentDate,IsClaimed,ClaimedDate,ClaimedEmail")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            db.Invitations.Remove(invitation);
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

        #region Helper Methods

        private void PopulateDropDown(object selected = null)
        {
            var units = from c in db.Organizations
                        orderby c.Name
                        select c;

            ViewBag.OrganizationId = new SelectList(units, "OrganizationId", "Name", selected);
        }

        private void AddMessage(string alertType, string message)
        {
            messages.Add(new MessageViewModel(){ AlertType = alertType, Message = message });
        }

        private void AddWarning(string message)
        {
            AddMessage("Warning", message);
        }

        private void AddInfo(string message)
        {
            AddMessage("Info", message);
        }

        private void AddDanger(string message)
        {
            AddMessage("Danger", message);
        }

        private void AddSuccess(string message)
        {
            AddMessage("Success", message);
        }

        private ActionResult Confirmation()
        {
            return View("Confirmation", messages);
        }

        #endregion
    }
}
