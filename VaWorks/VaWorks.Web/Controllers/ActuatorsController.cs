using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VaWorks.Web.Data;
using VaWorks.Web.Data.DataReaders;
using VaWorks.Web.Data.DataWriters;
using VaWorks.Web.Data.Entities;
using VaWorks.Web.ViewModels;

namespace VaWorks.Web.Controllers
{
    [Authorize(Roles = "System Administrator")]
    public class ActuatorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Actuators
        public ActionResult Index()
        {
            var actuators = db.Actuators.Include(v => v.InterfaceCodeEntity);
            return View(actuators.ToList());
        }

        public ActionResult Import(int? organizationId)
        {
            var organization = db.Organizations.Find(organizationId);
            PopulateDropDown((int)organizationId);
            return View(organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportActuators(int organizationId, HttpPostedFileBase file)
        {
            Dictionary<string, MessageViewModel> messages = new Dictionary<string, MessageViewModel>();

            if (file.ContentLength > 0) {
                try {
                    CSVReader reader = new CSVReader(file.InputStream, false, '\t');

                    IDataWriter writer = new ActuatorDataWriter(db, organizationId);
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
        public ActionResult CopyActuators(int? organizationId, int? fromOrganizationId)
        {
            Dictionary<string, MessageViewModel> messages = new Dictionary<string, MessageViewModel>();

            var fromOrganization = db.Organizations.Find(fromOrganizationId);
            var toOrganization = db.Organizations.Find(organizationId);

            foreach (var a in fromOrganization.Actuators) {
                toOrganization.Actuators.Add(a);
            }

            int result = db.SaveChanges();

            messages.Add("success", new MessageViewModel() {
                Message = $"{result} records affected",
                AlertType = "Success"
            });

            ViewBag.Id = organizationId;
            return View("ImportActuators", messages.Values);
        }

        public ActionResult List(int? organizationId)
        {
            var actuators = from v in db.Actuators
                         where v.Organizations.Any(o => o.OrganizationId == organizationId)
                         select v;

            return View(actuators);
        }

        // GET: Actuators/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actuator valve = db.Actuators.Find(id);
            if (valve == null)
            {
                return HttpNotFound();
            }
            return View(valve);
        }

        // GET: Actuators/Create
        public ActionResult Create()
        {
            ViewBag.InterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode");
            return View();
        }

        // POST: Actuators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActuatorId,InterfaceCode,Manufacturer,Model,Size")] Actuator valve)
        {
            if (ModelState.IsValid)
            {
                db.Actuators.Add(valve);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode", valve.InterfaceCode);
            return View(valve);
        }

        // GET: Actuators/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actuator valve = db.Actuators.Find(id);
            if (valve == null)
            {
                return HttpNotFound();
            }
            ViewBag.InterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode", valve.InterfaceCode);
            return View(valve);
        }

        // POST: Actuators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActuatorId,InterfaceCode,Manufacturer,Model,Size")] Actuator valve)
        {
            if (ModelState.IsValid)
            {
                db.Entry(valve).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InterfaceCode = new SelectList(db.ActuatorInterfaceCodes, "InterfaceCode", "InterfaceCode", valve.InterfaceCode);
            return View(valve);
        }

        // GET: Actuators/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Actuator valve = db.Actuators.Find(id);
            if (valve == null)
            {
                return HttpNotFound();
            }
            return View(valve);
        }

        // POST: Actuators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Actuator valve = db.Actuators.Find(id);
            db.Actuators.Remove(valve);
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
