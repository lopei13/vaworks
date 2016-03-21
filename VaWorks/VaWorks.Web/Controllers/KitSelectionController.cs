using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VaWorks.Web.Data;

namespace VaWorks.Web.Controllers
{
    [Authorize]
    public class KitSelectionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: KitSelection
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            return View(user);
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
        public JsonResult GetValveManufacturers()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var valves = from v in organization.Valves
                         join k in organization.Kits on v.InterfaceCode equals k.ValveInterfaceCode
                         select v.Manufacturer;

            return Json(valves.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the valve models
        /// </summary>
        /// <param name="mfg"></param>
        /// <returns></returns>
        public JsonResult GetValveModels(string mfg)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var valves = from v in organization.Valves
                         join k in organization.Kits on v.InterfaceCode equals k.ValveInterfaceCode
                         where v.Manufacturer == mfg
                         select v.Model;

            return Json(valves.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the valve sizes
        /// </summary>
        /// <param name="mfg"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetValveSizes(string mfg, string model)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var valves = from v in organization.Valves
                         join k in organization.Kits on v.InterfaceCode equals k.ValveInterfaceCode
                         where v.Manufacturer == mfg
                         where v.Model == model
                         select new { v.Size, v.InterfaceCode, v.ValveId };

            return Json(valves.OrderBy(x => x.Size).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the actuator manufacturers
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <returns></returns>
        public JsonResult GetActuators(int valveInterface)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var actuators = from a in organization.Actuators
                            join k in organization.Kits on a.InterfaceCode equals k.ActuatorInterfaceCode
                            where k.ValveInterfaceCode == valveInterface
                            select a.Manufacturer;

            return Json(actuators.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the actuator models
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <returns></returns>
        public JsonResult GetActuatorModels(int valveInterface, string mfg)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var actuators = from a in organization.Actuators
                            join k in organization.Kits on a.InterfaceCode equals k.ActuatorInterfaceCode
                            where k.ValveInterfaceCode == valveInterface
                            where a.Manufacturer == mfg
                            select a.Model;

            return Json(actuators.OrderBy(x => x).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the actuator sizes
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="mfg"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetActuatorSizes(int valveInterface, string mfg, string model)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var actuators = from a in organization.Actuators
                            join k in organization.Kits on a.InterfaceCode equals k.ActuatorInterfaceCode
                            where k.ValveInterfaceCode == valveInterface
                            where a.Manufacturer == mfg
                            where a.Model == model
                            select new { a.Size, a.InterfaceCode, a.ActuatorId };

            return Json(actuators.OrderBy(x => x.Size).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kit materials
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="actuatorInterface"></param>
        /// <returns></returns>
        public JsonResult GetKitMaterials(int valveInterface, int actuatorInterface)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var materials = from k in organization.Kits
                            where k.ValveInterfaceCode == valveInterface
                            where k.ActuatorInterfaceCode == actuatorInterface
                            select new { k.Material.Name, k.KitMaterialId };

            return Json(materials.OrderBy(x => x.Name).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kit options
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="actuatorInterface"></param>
        /// <returns></returns>
        public JsonResult GetKitOptions(int valveInterface, int actuatorInterface, int materialId)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var options = from k in organization.Kits
                          where k.ValveInterfaceCode == valveInterface
                          where k.ActuatorInterfaceCode == actuatorInterface
                          where k.KitMaterialId == materialId
                          select new { k.Option.Name, k.KitOptionId };

            return Json(options.OrderBy(x => x.Name).Distinct(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the kit
        /// </summary>
        /// <param name="valveInterface"></param>
        /// <param name="actuatorInterface"></param>
        /// <returns></returns>
        public JsonResult GetKit(int valveInterface, int actuatorInterface, int materialId, int optionId)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var organization = db.Organizations.Find(user.OrganizationId);

            var kit = from k in organization.Kits
                      where k.ValveInterfaceCode == valveInterface
                      where k.ActuatorInterfaceCode == actuatorInterface
                      where k.KitMaterialId == materialId
                      where k.KitOptionId == optionId
                      select new { k.KitNumber, k.KitId };

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
    }
}