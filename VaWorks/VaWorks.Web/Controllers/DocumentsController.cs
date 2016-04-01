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
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Documents
        public ActionResult Index()
        {
            return View(db.Documents.ToList());
        }

        public ActionResult Download(int documentId)
        {
            var document = db.Documents.Find(documentId);

            if(document != null) {
                return File(document.FileData, document.ContentType, document.FileName);
            }

            return HttpNotFound();
        }

        // GET: Documents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document documents = db.Documents.Find(id);
            if (documents == null)
            {
                return HttpNotFound();
            }
            return View(documents);
        }

        // GET: Documents/Create
        public ActionResult Create(int? organizationId)
        {
            ViewBag.OrganizationId = organizationId;
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Document document, HttpPostedFileBase file, int? organizationId)
        {
            if (ModelState.IsValid)
            {
                if(file != null) {
                    if(file.ContentLength > 0) {
                        byte[] data = new byte[file.ContentLength];
                        file.InputStream.Read(data, 0, file.ContentLength);

                        document.FileData = data;
                        document.ContentType = file.ContentType;
                        document.FileName = file.FileName;
                    }
                }

                if (string.IsNullOrEmpty(document.Name)) {
                    document.Name = document.FileName;
                }

                db.Documents.Add(document);

                if(organizationId != null) {
                    var org = db.Organizations.Find(organizationId);
                    if(org != null) {
                        org.Documents.Add(document);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(document);
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document documents = db.Documents.Find(id);
            if (documents == null)
            {
                return HttpNotFound();
            }
            return View(documents);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Document document, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;

                if (file != null) {
                    if (file.ContentLength > 0) {
                        byte[] data = new byte[file.ContentLength];
                        file.InputStream.Read(data, 0, file.ContentLength);

                        document.FileData = data;
                        document.ContentType = file.ContentType;
                        document.FileName = file.FileName;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(document);
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Link(int organizationId)
        {
            ViewBag.OrganizationId = organizationId;
            return View(db.Documents.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Link(int organizationId, int documentId)
        {
            var org = db.Organizations.Find(organizationId);
            var doc = db.Documents.Find(documentId);

            org.Documents.Add(doc);
            db.SaveChanges();

            return RedirectToAction("Details", "Organizations", new { id = organizationId });
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
