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
    }

    [Table("OrganizationKits")]
    public class OrganizationKits
    {
        [Key]
        [Column(Order = 1)]
        public int OrganizationId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int KitId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        
        [ForeignKey("KitId")]
        public virtual Kit Kit { get; set; }
    }

    [Table("OrganizationValves")]
    public class OrganizationValves
    {
        [Key]
        [Column(Order = 1)]
        public int OrganizationId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ValveId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [ForeignKey("ValveId")]
        public virtual Valve Valve { get; set; }
    }

    [Table("OrganizationActuators")]
    public class OrganizationActuators
    {
        [Key]
        [Column(Order = 1)]
        public int OrganizationId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ActuatorId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [ForeignKey("ActuatorId")]
        public virtual Actuator Actuator { get; set; }
    }

    [Table("OrganizationKitMaterials")]
    public class OrganizationKitMaterials
    {
        [Key]
        [Column(Order = 1)]
        public int OrganizationId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int KitMaterialId { get; set; }  

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [ForeignKey("KitMaterialId")]
        public virtual KitMaterial Material { get; set; }
    }
}