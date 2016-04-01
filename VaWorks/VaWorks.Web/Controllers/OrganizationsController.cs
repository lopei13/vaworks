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
    [Authorize(Roles = "System Administrator")]
    public class OrganizationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Organization
        public ActionResult Index()
        {
            var Organizations = db.Organizations.Where(b => b.ParentId == null).Include(b => b.Children);
            return View(Organizations.ToList());
        }

        // GET: Organization/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization Organization = db.Organizations.Find(id);
            if (Organization == null)
            {
                return HttpNotFound();
            }
            PopulateDropDown(Organization.ParentId);
            return View(Organization);
        }

        // GET: Organization/Create
        public ActionResult Create(int? parentId = null)
        {
            PopulateDropDown(parentId);
            return View();
        }

        // POST: Organization/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Organization Organization)
        {
            if (ModelState.IsValid)
            {
                db.Organizations.Add(Organization);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = Organization.OrganizationId });
            }

            PopulateDropDown(Organization.ParentId);
            return View(Organization);
        }

        // GET: Organization/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization Organization = db.Organizations.Find(id);
            if (Organization == null)
            {
                return HttpNotFound();
            }

            PopulateDropDown(Organization.ParentId);
            return View(Organization);
        }

        // POST: Organization/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Organization Organization)
        {
            if (ModelState.IsValid)
            {
                // check to see if we are trying to move the item down the tree
                Stack<Organization> stack = new Stack<Organization>();
                stack.Push(Organization);

                while(stack.Any()) {
                    var next = stack.Pop();

                    if(next.ParentId == Organization.OrganizationId) {
                        ViewBag.Error = "You cannot move an organization to a decendant.";
                        PopulateDropDown(Organization.ParentId);
                        return View(Organization);
                    }
                    foreach (var c in db.Organizations.Where(b => b.ParentId == next.OrganizationId)) {
                        stack.Push(c);
                    }
                }

                db.Entry(Organization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateDropDown(Organization.ParentId);
            return View(Organization);
        }

        // GET: Organization/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization Organization = db.Organizations.Find(id);
            if (Organization == null)
            {
                return HttpNotFound();
            }
            return View(Organization);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Organization Organization = db.Organizations.Find(id);
            db.Organizations.Remove(Organization);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveDocument(int organizationId, int documentId)
        {
            var org = db.Organizations.Find(organizationId);
            var doc = db.Documents.Find(documentId);

            org.Documents.Remove(doc);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = organizationId });
        }

        public ActionResult RemoveKit(int kitId, int organizationId)
        {
            var kit = db.Kits.Find(kitId);
            ViewBag.OrganizationId = organizationId;
            return View(kit);
        }

        [HttpPost, ActionName("RemoveKit")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveKitConfirmed(int kitId, int organizationId)
        {
            var organization = db.Organizations.Find(organizationId);
            var kit = db.Kits.Find(kitId);
            organization.Kits.Remove(kit);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = organizationId });
        }

        public ActionResult RemoveAllKits(int organizationId)
        {
            ViewBag.OrganizationId = organizationId;
            return View();
        }

        [HttpPost, ActionName("RemoveAllKits")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAllKitsConfirmed(int organizationId)
        {
            var organization = db.Organizations.Find(organizationId);
            organization.Kits.Clear();
            db.SaveChanges();
            return RedirectToAction("Details", new { id = organizationId });
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

            ViewBag.ParentId = new SelectList(units, "OrganizationId", "Name", selected);
        }

        #endregion
    }
}
