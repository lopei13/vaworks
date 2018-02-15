using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VaWorks.Web.Data;
using VaWorks.Web.Data.Entities;

namespace VaWorks.Web.Controllers
{
    [Authorize]
    public class KitSelectionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: KitSelection
        public ActionResult Index(int? organizationId)
        {
            if (organizationId == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                return View(user.OrganizationId);
            } else {
                return View(organizationId);
            }
        }

        public ActionResult Search(string searchText)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var kits = from k in organization.Kits
                       where k.KitNumber.Contains(searchText.ToUpper())
                       select k;

            ViewBag.SearchText = searchText;

            return View(kits);
        }

        /// <summary>
        /// Gets the valve manufactureres
        /// </summary>
        /// <returns></returns>
        public JsonResult GetValveManufacturers(int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            } 

            string sql = "select v.Manufacturer from OrganizationValves as ov " + 
                         "inner join Valves as v on v.ValveId = ov.ValveId " + 
                         "where v.InterfaceCode IN (select k.ValveInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId) and " +
                         "ov.OrganizationId = @organizationId";

            var valves = db.Database.SqlQuery<string>(sql, new SqlParameter("@organizationId", organizationId));          

            return Json(valves.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the valve models
        /// </summary>
        /// <param name="mfg"></param>
        /// <returns></returns>
        public JsonResult GetValveModels(string mfg, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select v.Model from OrganizationValves as ov " +
                         "inner join Valves as v on v.ValveId = ov.ValveId " +
                         "where v.Manufacturer = @mfg and ov.OrganizationId = @organizationId and v.InterfaceCode IN (select k.ValveInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId)";

            var valves = db.Database.SqlQuery<string>(sql, 
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@mfg", mfg));

            //var valves = from v in organization.Valves
            //             where organization.Kits.Select(k => k.ValveInterfaceCode).Contains(v.InterfaceCode)
            //             where v.Manufacturer == mfg
            //             select v.Model;

            return Json(valves.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the valve sizes
        /// </summary>
        /// <param name="mfg"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetValveSizes(string mfg, string model, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select v.Manufacturer, v.Model, v.Size, v.InterfaceCode, v.ValveId from OrganizationValves as ov " +
                         "inner join Valves as v on v.ValveId = ov.ValveId " +
                         "where v.Manufacturer = @mfg and v.Model = @model and ov.OrganizationId = @organizationId and v.InterfaceCode IN (select k.ValveInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId)";

            var valves = db.Database.SqlQuery<Valve>(sql,
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@mfg", mfg),
                new SqlParameter("@model", model)).ToList();

            return Json(valves.OrderBy(x => x.Size).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the actuator manufacturers
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <returns></returns>
        public JsonResult GetActuators(int valveInterface, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select a.Manufacturer from OrganizationActuators as oa " +
                         "inner join Actuators as a on a.ActuatorId = oa.ActuatorId " +
                         "where oa.OrganizationId = @organizationId and a.InterfaceCode IN (select k.ActuatorInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId where k.ValveInterfaceCode = @valveInterfaceCode)";

            var actuators = db.Database.SqlQuery<string>(sql,
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@valveInterfaceCode", valveInterface));

            //var actuators = from a in organization.Actuators
            //                where organization.Kits.Where(k => k.ValveInterfaceCode == valveInterface)
            //                .Select(k => k.ActuatorInterfaceCode).Contains(a.InterfaceCode)
            //                select a.Manufacturer;

            return Json(actuators.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the actuator models
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <returns></returns>
        public JsonResult GetActuatorModels(int valveInterface, string mfg, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select a.Model from OrganizationActuators as oa " +
                         "inner join Actuators as a on a.ActuatorId = oa.ActuatorId " +
                         "where a.Manufacturer = @mfg and oa.OrganizationId = @organizationId and a.InterfaceCode IN (select k.ActuatorInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId where k.ValveInterfaceCode = @valveInterfaceCode)";

            var actuators = db.Database.SqlQuery<string>(sql,
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@valveInterfaceCode", valveInterface),
                new SqlParameter("@mfg", mfg));

            //var actuators = from a in organization.Actuators
            //                where organization.Kits.Where(k => k.ValveInterfaceCode == valveInterface)
            //                .Select(k => k.ActuatorInterfaceCode).Contains(a.InterfaceCode)
            //                where a.Manufacturer == mfg
            //                select a.Model;

            return Json(actuators.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the actuator sizes
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="mfg"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetActuatorSizes(int valveInterface, string mfg, string model, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select a.Manufacturer, a.Model, a.Size, a.InterfaceCode, a.ActuatorId from OrganizationActuators as oa " +
                         "inner join Actuators as a on a.ActuatorId = oa.ActuatorId " +
                         "where a.Manufacturer = @mfg and a.Model = @model and oa.OrganizationId = @organizationId and a.InterfaceCode IN (select k.ActuatorInterfaceCode from OrganizationKits as ok join Kits as k on k.KitId = ok.KitId where k.ValveInterfaceCode = @valveInterfaceCode)";

            var actuators = db.Database.SqlQuery<Actuator>(sql,
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@valveInterfaceCode", valveInterface),
                new SqlParameter("@mfg", mfg),
                new SqlParameter("@model", model)).ToList();

            //var actuators = from a in organization.Actuators
            //                where organization.Kits.Where(k => k.ValveInterfaceCode == valveInterface)
            //                .Select(k => k.ActuatorInterfaceCode).Contains(a.InterfaceCode)
            //                where a.Manufacturer == mfg
            //                where a.Model == model
            //                select new { a.Size, a.InterfaceCode, a.ActuatorId };

            return Json(actuators.OrderBy(x => x.Size).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kit materials
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="actuatorInterface"></param>
        /// <returns></returns>
        public JsonResult GetKitMaterials(int valveInterface, int actuatorInterface, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select distinct km.KitMaterialId, km.Name, km.SortOrder, km.Code from KitMaterials as km " +
                         "inner join Kits as k on k.KitMaterialId = km.KitMaterialId " +
                         "inner join OrganizationKits as ok on k.KitId = ok.KitId " +
                         "where k.ValveInterfaceCode = @valveInterfaceCode and k.ActuatorInterfaceCode = @actuatorInterfaceCode and ok.OrganizationId = @organizationId";

            var materials = db.Database.SqlQuery<KitMaterial>(sql,
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@valveInterfaceCode", valveInterface),
                new SqlParameter("@actuatorInterfaceCode", actuatorInterface)).ToList();

            return Json(materials.OrderBy(x => x.SortOrder).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kit options
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="actuatorInterface"></param>
        /// <returns></returns>
        public JsonResult GetKitOptions(int valveInterface, int actuatorInterface, int materialId, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select distinct ko.KitOptionId, ko.Name, ko.SortOrder, ko.Code from KitOptions as ko " +
                         "inner join Kits as k on k.KitOptionId = ko.KitOptionId " +
                         "inner join OrganizationKits as ok on k.KitId = ok.KitId " +
                         "where k.KitMaterialId = @kitMaterialId and k.ValveInterfaceCode = @valveInterfaceCode and k.ActuatorInterfaceCode = @actuatorInterfaceCode and ok.OrganizationId = @organizationId";

            var options = db.Database.SqlQuery<KitOption>(sql,
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@valveInterfaceCode", valveInterface),
                new SqlParameter("@actuatorInterfaceCode", actuatorInterface),
                new SqlParameter("@kitMaterialId", materialId)).ToList();

            //var options = from k in organization.Kits
            //              where k.ValveInterfaceCode == valveInterface
            //              where k.ActuatorInterfaceCode == actuatorInterface
            //              where k.KitMaterialId == materialId
            //              select new { k.Option.Name, k.KitOptionId };

            return Json(options.OrderBy(x => x.SortOrder).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kit
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="actuatorInterface"></param>
        /// <returns></returns>
        public JsonResult GetKit(int valveInterface, int actuatorInterface, int materialId, int optionId, int? organizationId)
        {
            var id = organizationId;
            if (id == null) {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                id = user.OrganizationId;
            }

            string sql = "select * from Kits as k " +
                         "inner join OrganizationKits as ok on k.KitId = ok.KitId " +
                         "where k.KitMaterialId = @kitMaterialId and " + 
                         "k.ValveInterfaceCode = @valveInterfaceCode and " + 
                         "k.ActuatorInterfaceCode = @actuatorInterfaceCode and " + 
                         "k.KitOptionId = @optionId and " + 
                         "ok.OrganizationId = @organizationId";

            var kit = db.Database.SqlQuery<Kit>(sql,
                new SqlParameter("@organizationId", organizationId),
                new SqlParameter("@valveInterfaceCode", valveInterface),
                new SqlParameter("@actuatorInterfaceCode", actuatorInterface),
                new SqlParameter("@optionId", optionId),
                new SqlParameter("@kitMaterialId", materialId)).ToList();

            //var kit = from k in organization.Kits
            //          where k.ValveInterfaceCode == valveInterface
            //          where k.ActuatorInterfaceCode == actuatorInterface
            //          where k.KitMaterialId == materialId
            //          where k.KitOptionId == optionId
            //          select new { k.KitNumber, k.KitId };

            return Json(kit, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds current item to shopping cart.
        /// </summary>
        /// <param name="valveId"></param>
        /// <param name="actuatorId"></param>
        /// <param name="kitId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public ActionResult AddToCart(int valveId, int actuatorId, int kitId, int quantity, int? quoteId)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            if (quoteId == null) {
                db.ShoppingCartItems.Add(new Data.Entities.ShoppingCartItems() {
                    ActuatorId = actuatorId,
                    ValveId = valveId,
                    Quantity = quantity,
                    KitId = kitId,
                    UserId = user.Id
                });

                db.SaveChanges();
                return RedirectToAction("Index", "ShoppingCartItems");
            } else {
                var quote = db.Quotes.Find(quoteId);
                var customer = db.Users.Find(quote.CustomerId);
                var org = customer.Organization;
                var dis = org.Discounts.Where(d => d.Quantity < quantity).OrderBy(d => d.Quantity).FirstOrDefault();
                double discount = 1;
                if (dis != null) {
                    discount = dis.DiscountPercentage / 100;
                }
                var kit = db.Kits.Find(kitId);
                var valve = db.Valves.Find(valveId);
                var actuator = db.Actuators.Find(actuatorId);
                double listPrice = kit.Price;

                quote.Items.Add(new Data.Entities.QuoteItem() {
                    Discount = discount,
                    PriceEach = listPrice * discount,
                    Description = $"KIT FOR {actuator.ToString()} TO {valve.ToString()}",
                    KitNumber = kit.KitNumber,
                    Quantity = quantity,
                    Actuator = actuator.ToString(),
                    Valve = valve.ToString(),
                    TotalPrice = listPrice * discount * quantity
                });

                // update the price on the quote
                quote.Total = 0;
                quote.Total = quote.Items.Sum(q => q.TotalPrice);

                db.SaveChanges();
                return RedirectToAction("Edit", "Quotes", new { id = quoteId });
            }            
        }

        //public ActionResult ViewDrawing(int valveId, int actuatorId, int kitId)
        //{
        //    var kit = (from k in db.Kits
        //              join v in db.Valves on k.ValveInterfaceCode equals v.InterfaceCode
        //              join a in db.Actuators on k.ActuatorInterfaceCode equals a.InterfaceCode
        //              where v.ValveId == valveId && a.ActuatorId == actuatorId && k.KitId == kitId
        //              select new ViewModels.KitDrawingViewModel() {
        //                  KitNumber = k.KitNumber,
        //                  Description = $"KIT FOR {a.Manufacturer} {a.Model} {a.Size} TO A {v.Size} {v.Manufacturer} {v.Model}"
        //              }).FirstOrDefault();

        //    if (kit != null) {
        //        return View(kit);
        //    } else {
        //        return HttpNotFound();
        //    }
        //}

        public ActionResult ViewDrawing(string kitNumber, string description)
        {
            return View(new ViewModels.KitDrawingViewModel() {
                KitNumber = kitNumber,
                Description = description
            });
        }
    }
}