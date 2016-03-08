using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using VaWorks.Web.DataAccess.Entities;

namespace VaWorks.Web.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region DbSets

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Kit> Kits { get; set; }

        public DbSet<Valve> Valves { get; set; }

        public DbSet<Actuator> Actuators { get; set; }

        public DbSet<OrganizationActuators> OrganizationActuators { get; set; }

        public DbSet<OrganizationValves> OrganizationValves { get; set; }

        public DbSet<OrganizationKits> OrganizationKits { get; set; }

        public DbSet<ValveInterfaceCode> ValveInterfaceCodes { get; set; }

        public DbSet<ActuatorInterfaceCode> ActuatorInterfaceCodes { get; set; }

        public DbSet<KitMaterial> KitMaterials { get; set; }

        public DbSet<KitOption> KitOptions { get; set; }

        #endregion


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}