using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VaWorks.Web.Data.Entities;

namespace VaWorks.Web.Data
{
    public class ReleaseInitializer : System.Data.Entity.CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));


            if (!roleManager.RoleExists("System Administrator")) {
                roleManager.Create(new IdentityRole("System Administrator"));
            }

            var admin = new ApplicationUser() {
                Name = "System Administrator",
                Email = "tlambert@vanaireinc.com",
                UserName = "admin"
            };
            var adminResult = userManager.Create(admin, "VanAire1125");

            if (adminResult.Succeeded) {
                var result = userManager.AddToRole(admin.Id, "System Administrator");
            }

            if (context.Organizations.Count() == 0) {
                Organization vanaire = new Organization() {
                    Name = "VanAire"
                };

                context.Organizations.Add(vanaire);

                admin.Organization = vanaire;

                context.SaveChanges();
            }
        }

        public override void InitializeDatabase(ApplicationDbContext context)
        {
            base.InitializeDatabase(context);
        }
    }
}