using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VaWorks.Web.DataAccess;
using VaWorks.Web.DataAccess.Entities;
using VaWorks.Web.Models;

namespace VaWorks.Web.Controllers
{
    public class KitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Kits
        public ActionResult Index()
        {
            var kits = db.Kits;
            return View(kits.ToList());
        }

        public ActionResult Import(int businessId)
        {
            var businessUnit = db.BusinessUnits.Find(businessId);
            PopulateDropDown(businessId);
            return View(businessUnit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportKits(int businessUnitId, HttpPostedFileBase file, bool overwrite = false)
        {
            List<MessageViewModel> messages = new List<MessageViewModel>();
            int count = 0;
            if (file.ContentLength > 0) {
                try {
                    TextReader reader = new StreamReader(file.InputStream);
                    while (reader.Peek() > 0) {
                        string[] line = reader.ReadLine().Split('\t');

                        count++;
                    }
                } catch { }
            }

            messages.Add(new MessageViewModel() {
                Message = $"{count} kits inserted into the database",
                AlertType = "Success"
            });

            messages.Add(new MessageViewModel() {
                Message = "17 kits failed to import",
                AlertType = "Danger"
            });

            messages.Add(new MessageViewModel() {
                Message = "34 kits already existed",
                AlertType = "Warning"
            });
            ViewBag.Id = businessUnitId;
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CopyKits(int businessUnitId, int fromBusinessUnitId, bool overwrite = false)
        {
            List<MessageViewModel> messages = new List<MessageViewModel>();

            var copyFrom = db.BusinessUnits.Find(fromBusinessUnitId);
            var copyTo = db.BusinessUnits.Find(businessUnitId);

            messages.Add(new MessageViewModel() {
                Message = $"Kits copied from {copyFrom.Name} to {copyTo.Name}",
                AlertType = "Success"
            });
            ViewBag.Id = businessUnitId;
            return View("ImportKits", messages);
        }

        // GET: Kits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kit kit = db.Kits.Find(id);
            if (kit == null)
            {
                return HttpNotFound();
            }
            return View(kit);
        }

        // GET: Kits/Create
        public ActionResult Create()
        {
            ViewBag.ActuatorInterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode");
            ViewBag.ValveInterfaceCode = new SelectList(db.ValveInterfaceCodes, "InterfaceCode", "InterfaceCode");
            return View();
        }

        // POST: Kits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KitId,BaseNumber,ValveInterfaceCode,ActuatorInterfaceCode")] Kit kit)
        {
            if (ModelState.IsValid)
            {
                db.Kits.Add(kit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActuatorInterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode", kit.ActuatorInterfaceCode);
            ViewBag.ValveInterfaceCode = new SelectList(db.ValveInterfaceCodes, "InterfaceCode", "InterfaceCode", kit.ValveInterfaceCode);
            return View(kit);
        }

        // GET: Kits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kit kit = db.Kits.Find(id);
            if (kit == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActuatorInterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode", kit.ActuatorInterfaceCode);
            ViewBag.ValveInterfaceCode = new SelectList(db.ValveInterfaceCodes, "InterfaceCode", "InterfaceCode", kit.ValveInterfaceCode);
            return View(kit);
        }

        // POST: Kits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KitId,BaseNumber,ValveInterfaceCode,ActuatorInterfaceCode")] Kit kit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActuatorInterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode", kit.ActuatorInterfaceCode);
            ViewBag.ValveInterfaceCode = new SelectList(db.ValveInterfaceCodes, "InterfaceCode", "InterfaceCode", kit.ValveInterfaceCode);
            return View(kit);
        }

        // GET: Kits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kit kit = db.Kits.Find(id);
            if (kit == null)
            {
                return HttpNotFound();
            }
            return View(kit);
        }

        // POST: Kits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kit kit = db.Kits.Find(id);
            db.Kits.Remove(kit);
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

        private void PopulateDropDown(int exclude, object selected = null)
        {
            var units = from c in db.BusinessUnits
                        where c.BusinessUnitId != exclude
                        orderby c.Name
                        select c;

            ViewBag.FromBusinessUnitId = new SelectList(units, "BusinessUnitId", "Name", selected);
        }

        #endregion
    }
}
