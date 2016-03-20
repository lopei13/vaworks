using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using VaWorks.Web.Data.Entities;

namespace VaWorks.Web.Data
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

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<InvitationRequest> InvitationRequests { get; set; }

        public DbSet<ShoppingCartItems> ShoppingCartItems { get; set; }

        public DbSet<Quote> Quotes { get; set; }

        public DbSet<QuoteItem> QuoteItems { get; set; }

        public DbSet<Discount> Discounts { get; set; }

        public DbSet<SystemMessage> SystemMessages { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<QuoteNumber> QuoteNumber { get; set; }

        #endregion


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ReleaseInitializer());
            var o = this.Organizations.FirstOrDefault();
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

            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(u => u.ShoppingCart)
                .WithOptionalDependent()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Contacts)
                .WithMany()
                .Map(m => {
                    m.ToTable("Contacts");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("ContactId");
                });

            modelBuilder.Entity<Organization>()
                .HasMany(o => o.Documents)
                .WithMany(d => d.Organizations)
                .Map(x => {
                    x.ToTable("OrganizationDocuments");
                    x.MapLeftKey("OrganizationId");
                    x.MapRightKey("DocumentId");
                });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Messages)
                .WithRequired(m => m.User)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

    }
}