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
using VaWorks.Web.Data.Entities;
using VaWorks.Web.Data.DataReaders;
using VaWorks.Web.Data.DataWriters;
using VaWorks.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

namespace VaWorks.Web.Controllers
{
    [Authorize(Roles = "System Administrator")]
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

                } catch (Exception ex) {

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

        public ActionResult UploadThumbnails()
        {
            return View();
        }

        public ActionResult UploadThumbnailHandler()
        {
            var file = Request.Files["Filedata"];
            string savePath = Server.MapPath(@"~\Content\Thumbnails\" + file.FileName);
            file.SaveAs(savePath);

            return Json(new { name = file.FileName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadDrawings()
        {
            return View();
        }

        public ActionResult UploadDrawingHandler()
        {
            var file = Request.Files["Filedata"];
            string savePath = Server.MapPath(@"~\Content\Drawings\" + file.FileName);
            file.SaveAs(savePath);

            return Json(new { name = file.FileName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(int? organizationId)
        {
            ViewBag.OrganizationId = organizationId;
            return View();
        }

        public ActionResult GetKits(string searchText, int? organizationId)
        {
            if (!searchText.ToUpper().StartsWith("VA")) {
                searchText = "VA" + searchText;
            }
            if (searchText.Length >= 6) {

                var id = organizationId;
                if (id == null) {
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Find(userId);
                    id = user.OrganizationId;
                }

                string sql = "select k.KitId, k.KitNumber, k.Price, k.ActuatorInterfaceCode, k.ValveInterfaceCode, k.KitOptionId, k.KitMaterialId from Kits as k " +
                             "inner join OrganizationKits as ok on k.KitId = ok.KitId " +
                             "where k.KitNumber LIKE @searchText and ok.OrganizationId = @organizationId";

                var kits = db.Database.SqlQuery<Kit>(sql,
                    new SqlParameter("@searchText", searchText + "%"),
                    new SqlParameter("@organizationId", organizationId)).ToList();

                ViewBag.OrganizationId = organizationId;
                return Json(kits, JsonRequestBehavior.AllowGet);
            }

            return HttpNotFound();
        }
    

        public ActionResult SelectResult(int kitId, int organizationId)
        {
            var kit = db.Kits.Find(kitId);

            ViewBag.OrganizationId = organizationId;

            return View(kit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectResult(int kitId, int organizationId, int valveId, int actuatorId)
        {
            var kit = db.Kits.Find(kitId);
            ViewBag.OrganizationId = organizationId;
            var valve = db.Valves.Find(valveId);
            var actuator = db.Actuators.Find(actuatorId);

            return View("KitDetails", new { Kit = kit, Valve = valve, Actuator = actuator });
        }

        public JsonResult GetValves(int kitId, int organizationId)
        {
            string sql = "select v.Manufacturer, v.Model, v.Size, v.InterfaceCode, v.ValveId from OrganizationValves as ov " +
                        "inner join Valves as v on v.ValveId = ov.ValveId " +
                        "where v.InterfaceCode IN (select k.ValveInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId where ok.KitId = @kitId) and " +
                        "ov.OrganizationId = @organizationId";

            var valves = db.Database.SqlQuery<Valve>(sql,
                new SqlParameter("@kitId", kitId),
                new SqlParameter("@organizationId", organizationId)).ToList();

            return Json(valves, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActuators(int kitId, int organizationId)
        {
            string sql = "select a.Manufacturer, a.Model, a.Size, a.InterfaceCode, a.ActuatorId from OrganizationActuators as oa " +
                            "inner join Actuators as a on a.ActuatorId = oa.ActuatorId " +
                            "where a.InterfaceCode IN (select k.ActuatorInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId where ok.KitId = @kitId) and " +
                            "oa.OrganizationId = @organizationId";

            var actuators = db.Database.SqlQuery<Actuator>(sql,
                new SqlParameter("@kitId", kitId),
                new SqlParameter("@organizationId", organizationId)).ToList();

            return Json(actuators, JsonRequestBehavior.AllowGet);
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
        public ActionResult Edit(Kit kit)
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

        [HttpPost]
        public ActionResult SavePrice(int kitId, double price)
        {
            var kit = db.Kits.Find(kitId);
            if(kit != null) {
                kit.Price = price;
                db.SaveChanges();

                return Json(kit, JsonRequestBehavior.AllowGet);
            }

            return HttpNotFound();
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

        public ActionResult ExportKits()
        {
            using (var writer = new StreamWriter(new MemoryStream())) {
                
                foreach(var k in db.Kits) {
                    writer.WriteLine(k.ToString());
                }
                byte[] data = new byte[writer.BaseStream.Length];
                writer.BaseStream.Read(data, 0, (int)writer.BaseStream.Length);
                return File(data, "text/csv", "kits.csv");
            }
        }

        public ActionResult UpdatePricing()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePricing(HttpPostedFileBase file)
        {
            CSVReader reader = new CSVReader(file.InputStream, false, '\t');

            while (reader.Read()) {
                string kitNumber = reader.GetString(0);

                Kit kit = db.Kits.Where(k => k.KitNumber == kitNumber).FirstOrDefault();
                if(kit != null) {

                    // get the price
                    try {
                        double price = reader.GetDouble(reader.FieldCount - 1);
                        kit.Price = price;
                    } catch { }
                }

            }

            int result = db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult KitsWithoutImages()
        {
            List<Kit> kits = new List<Kit>();
            foreach(var k in db.Kits) {
                if (!System.IO.File.Exists(Server.MapPath($"~/Content/Thumbnails/{k.KitNumber}.jpg"))) {
                    kits.Add(k);
                }
            }
            return View("Index", kits);
        }

        public ActionResult KitsWithoutDrawings()
        {
            List<Kit> kits = new List<Kit>();
            foreach (var k in db.Kits) {
                if (!System.IO.File.Exists(Server.MapPath($"~/Content/Drawings/{k.KitNumber}.pdf"))) {
                    kits.Add(k);
                }
            }
            return View("Index", kits);
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
