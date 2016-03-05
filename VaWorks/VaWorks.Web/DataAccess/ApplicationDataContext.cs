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