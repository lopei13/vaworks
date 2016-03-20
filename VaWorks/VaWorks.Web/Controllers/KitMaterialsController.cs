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
    public class KitMaterialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: KitMaterials
        public ActionResult Index()
        {
            return View(db.KitMaterials.ToList());
        }

        // GET: KitMaterials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KitMaterial kitMaterial = db.KitMaterials.Find(id);
            if (kitMaterial == null)
            {
                return HttpNotFound();
            }
            return View(kitMaterial);
        }

        // GET: KitMaterials/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KitMaterials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KitMaterialId,Name,Code,SortOrder")] KitMaterial kitMaterial)
        {
            if (ModelState.IsValid)
            {
                db.KitMaterials.Add(kitMaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kitMaterial);
        }

        // GET: KitMaterials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KitMaterial kitMaterial = db.KitMaterials.Find(id);
            if (kitMaterial == null)
            {
                return HttpNotFound();
            }
            return View(kitMaterial);
        }

        // POST: KitMaterials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KitMaterialId,Name,Code,SortOrder")] KitMaterial kitMaterial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kitMaterial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kitMaterial);
        }

        // GET: KitMaterials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KitMaterial kitMaterial = db.KitMaterials.Find(id);
            if (kitMaterial == null)
            {
                return HttpNotFound();
            }
            return View(kitMaterial);
        }

        // POST: KitMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KitMaterial kitMaterial = db.KitMaterials.Find(id);
            db.KitMaterials.Remove(kitMaterial);
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
