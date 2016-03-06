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

        public DbSet<BusinessUnit> BusinessUnits { get; set; }

        public DbSet<Kit> Kits { get; set; }

        public DbSet<Valve> Valves { get; set; }

        public DbSet<Actuator> Actuators { get; set; }

        public DbSet<BusinessUnitActuators> BusinessUnitActuators { get; set; }

        public DbSet<BusinessUnitValves> BusinessUnitValves { get; set; }

        public DbSet<BusinessUnitKits> BusinessUnitKits { get; set; }

        public DbSet<ValveInterfaceCode> ValveInterfaceCodes { get; set; }

        public DbSet<ActuatorInterfaceCode> ActuatorInterfaceCodes { get; set; }

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