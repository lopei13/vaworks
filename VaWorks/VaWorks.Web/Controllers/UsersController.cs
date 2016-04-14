using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using VaWorks.Web.Data;

namespace VaWorks.Web.Controllers
{
    [System.Web.Http.Authorize(Roles = "System Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var users = db.Users.OrderBy(u => u.Name);
            return View(users);
        }
    }
}
