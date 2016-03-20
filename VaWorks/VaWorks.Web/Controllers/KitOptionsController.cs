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
    public class KitOptionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: KitOptions
        public ActionResult Index()
        {
            return View(db.KitOptions.ToList());
        }

        // GET: KitOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KitOption kitOption = db.KitOptions.Find(id);
            if (kitOption == null)
            {
                return HttpNotFound();
            }
            return View(kitOption);
        }

        // GET: KitOptions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KitOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KitOptionId,Name,Code,SortOrder")] KitOption kitOption)
        {
            if (ModelState.IsValid)
            {
                db.KitOptions.Add(kitOption);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kitOption);
        }

        // GET: KitOptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KitOption kitOption = db.KitOptions.Find(id);
            if (kitOption == null)
            {
                return HttpNotFound();
            }
            return View(kitOption);
        }

        // POST: KitOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KitOptionId,Name,Code,SortOrder")] KitOption kitOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kitOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kitOption);
        }

        // GET: KitOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KitOption kitOption = db.KitOptions.Find(id);
            if (kitOption == null)
            {
                return HttpNotFound();
            }
            return View(kitOption);
        }

        // POST: KitOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KitOption kitOption = db.KitOptions.Find(id);
            db.KitOptions.Remove(kitOption);
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
    }
}
