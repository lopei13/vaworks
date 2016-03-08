using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.DataAccess.Entities
{
    [Table("Organizations")]
    public class Organization
    {
        [Key]
        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Parent Organization")]
        public int? ParentId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        [ForeignKey("ParentId")]
        public virtual Organization ParentOrganization { get; set; }

        public virtual ICollection<Organization> Children { get; set; }

        public virtual ICollection<Kit> Kits { get; set; }

        public virtual ICollection<Valve> Valves { get; set; }

        public virtual ICollection<Actuator> Actuators { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}