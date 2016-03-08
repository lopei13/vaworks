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
using VaWorks.Web.DataAccess.DataReaders;
using VaWorks.Web.Models;
using VaWorks.Web.DataAccess.DataWriters;

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

        public ActionResult Import(int organizationId)
        {
            var organization = db.Organizations.Find(organizationId);
            PopulateDropDown(organizationId);
            return View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportKits(int organizationId, HttpPostedFileBase file, bool overwrite = false)
        {
            Dictionary<string, MessageViewModel> messages = new Dictionary<string, MessageViewModel>();

            if (file.ContentLength > 0) {
                try {

                    var organization = db.Organizations.Find(organizationId);

                    CSVReader reader = new CSVReader(file.InputStream, false, '\t');

                    IDataWriter writer = new KitDataWriter(db, organizationId);
                    writer.Write(reader);
                    int result = writer.SaveChanges();

                    messages.Add("success", new MessageViewModel() {
                        Message = $"{result} records affected",
                        AlertType = "Success"
                    });

                } catch(Exception ex) {

                    if (!messages.ContainsKey(ex.Message)) {
                        messages.Add(ex.Message, new MessageViewModel() {
                            Message = ex.Message,
                            AlertType = "Danger"
                        });
                    }

                }
            }

            ViewBag.Id = organizationId;
            return View(messages.Values);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CopyKits(int? organizationId, int? fromOrganizationId)
        {
            Dictionary<string, MessageViewModel> messages = new Dictionary<string, MessageViewModel>();

            var fromOrganization = db.Organizations.Find(fromOrganizationId);
            var toOrganization = db.Organizations.Find(organizationId);

            foreach (var k in fromOrganization.Kits) {
                toOrganization.Kits.Add(k);
            }

            int result = db.SaveChanges();

            messages.Add("success", new MessageViewModel() {
                Message = $"{result} records affected",
                AlertType = "Success"
            });

            ViewBag.Id = organizationId;
            return View("ImportKits", messages.Values);
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
            var units = from c in db.Organizations
                        where c.OrganizationId != exclude
                        orderby c.Name
                        select c;

            ViewBag.FromOrganizationId = new SelectList(units, "OrganizationId", "Name", selected);
        }

        #endregion
    }
}
