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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>()
                .HasMany(x => x.Actuators)
                .WithMany(x => x.Organizations)
                .Map(x => {
                    x.ToTable("OrganizationActuators");
                    x.MapLeftKey("OrganizationId");
                    x.MapRightKey("ActuatorId");
                });

            modelBuilder.Entity<Organization>()
                .HasMany(x => x.Valves)
                .WithMany(x => x.Organizations)
                .Map(x => {
                    x.ToTable("OrganizationValves");
                    x.MapLeftKey("OrganizationId");
                    x.MapRightKey("ValveId");
                });

            modelBuilder.Entity<Organization>()
                .HasMany(x => x.Kits)
                .WithMany(x => x.Organizations)
                .Map(x => {
                    x.ToTable("OrganizationKits");
                    x.MapLeftKey("OrganizationId");
                    x.MapRightKey("KitId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}