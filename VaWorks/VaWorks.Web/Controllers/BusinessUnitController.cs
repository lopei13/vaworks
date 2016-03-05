using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VaWorks.Web.DataAccess;
using VaWorks.Web.DataAccess.Entities;

namespace VaWorks.Web.Controllers
{
    public class BusinessUnitController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BusinessUnit
        public ActionResult Index()
        {
            var businessUnits = db.BusinessUnits.Where(b => b.ParentBusinessUnitId == null).Include(b => b.Children);
            return View(businessUnits.ToList());
        }

        // GET: BusinessUnit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessUnit businessUnit = db.BusinessUnits.Find(id);
            if (businessUnit == null)
            {
                return HttpNotFound();
            }
            return View(businessUnit);
        }

        // GET: BusinessUnit/Create
        public ActionResult Create(int? parentId = null)
        {
            PopulateDropDown(parentId);
            return View();
        }

        // POST: BusinessUnit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BusinessUnitId,ParentBusinessUnitId,Name,Description")] BusinessUnit businessUnit)
        {
            if (ModelState.IsValid)
            {
                db.BusinessUnits.Add(businessUnit);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = businessUnit.BusinessUnitId });
            }

            PopulateDropDown(businessUnit.ParentBusinessUnitId);
            return View(businessUnit);
        }

        // GET: BusinessUnit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessUnit businessUnit = db.BusinessUnits.Find(id);
            if (businessUnit == null)
            {
                return HttpNotFound();
            }

            PopulateDropDown(businessUnit.ParentBusinessUnitId);
            return View(businessUnit);
        }

        // POST: BusinessUnit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BusinessUnitId,ParentBusinessUnitId,Name,Description")] BusinessUnit businessUnit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(businessUnit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateDropDown(businessUnit.ParentBusinessUnitId);
            return View(businessUnit);
        }

        // GET: BusinessUnit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessUnit businessUnit = db.BusinessUnits.Find(id);
            if (businessUnit == null)
            {
                return HttpNotFound();
            }
            return View(businessUnit);
        }

        // POST: BusinessUnit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BusinessUnit businessUnit = db.BusinessUnits.Find(id);
            db.BusinessUnits.Remove(businessUnit);
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
            var units = from c in db.BusinessUnits
                            orderby c.Name
                            select c;

            ViewBag.ParentBusinessUnitId = new SelectList(units, "BusinessUnitId", "Name", selected);
        }

        #endregion
    }
}
