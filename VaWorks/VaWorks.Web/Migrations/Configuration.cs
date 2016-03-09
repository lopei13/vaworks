namespace VaWorks.Web.Migrations
{
    using Data.Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<VaWorks.Web.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(VaWorks.Web.Data.ApplicationDbContext context)
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

            if(context.Organizations.Count() == 0) {
                context.Organizations.Add(new Organization() {
                    Name = "VanAire"
                });
            }

            context.SaveChanges();
        }
    }
}
