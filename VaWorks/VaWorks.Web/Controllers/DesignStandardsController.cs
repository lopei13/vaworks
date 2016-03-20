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
    public class DesignStandardsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DesignStandards
        public ActionResult Index()
        {
            return View(db.KitMaterials.ToList());
        }

        // GET: DesignStandards/Details/5
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

        public ActionResult Download(int id)
        {
            var material = db.KitMaterials.Find(id);

            if(material != null) {

                return File(material.FileData, material.ContentType, material.FileName);
            }
            return HttpNotFound();
        }

        // GET: DesignStandards/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DesignStandards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KitMaterialId,Name,Code,SortOrder,Description,FileData,ContentType")] KitMaterial kitMaterial, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null) {
                    if (file.ContentLength > 0) {
                        byte[] data = new byte[file.ContentLength];
                        file.InputStream.Read(data, 0, file.ContentLength);
                        kitMaterial.FileData = data;
                        kitMaterial.ContentType = file.ContentType;
                        kitMaterial.FileName = file.FileName;
                    }
                }

                db.KitMaterials.Add(kitMaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kitMaterial);
        }

        // GET: DesignStandards/Edit/5
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

        // POST: DesignStandards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KitMaterial kitMaterial, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kitMaterial).State = EntityState.Modified;

                if(file != null) {
                    if (file.ContentLength > 0) {
                        byte[] data = new byte[file.ContentLength];
                        file.InputStream.Read(data, 0, file.ContentLength);
                        kitMaterial.FileData = data;
                        kitMaterial.ContentType = file.ContentType;
                        kitMaterial.FileName = file.FileName;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kitMaterial);
        }

        // GET: DesignStandards/Delete/5
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

        // POST: DesignStandards/Delete/5
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
