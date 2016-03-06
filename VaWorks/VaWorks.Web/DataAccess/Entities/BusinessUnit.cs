using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.DataAccess.Entities
{
    [Table("BusinessUnits")]
    public class BusinessUnit
    {
        [Key]
        [Display(Name = "Business Unit")]
        public int BusinessUnitId { get; set; }

        [Display(Name = "Parent Business Unit")]
        public int? ParentBusinessUnitId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        [ForeignKey("ParentBusinessUnitId")]
        public virtual BusinessUnit ParentBusinessUnit { get; set; }

        public virtual ICollection<BusinessUnit> Children { get; set; }
    }

    [Table("BusinessUnitKits")]
    public class BusinessUnitKits
    {
        [Key]
        [Column(Order = 1)]
        public int BusinessUnitId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int KitId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public virtual BusinessUnit BusinessUnit { get; set; }
        
        [ForeignKey("KitId")]
        public virtual Kit Kit { get; set; }
    }

    [Table("BusinessUnitValves")]
    public class BusinessUnitValves
    {
        [Key]
        [Column(Order = 1)]
        public int BusinessUnitId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ValveId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public virtual BusinessUnit BusinessUnit { get; set; }

        [ForeignKey("ValveId")]
        public virtual Valve Valve { get; set; }
    }

    [Table("BusinessUnitActuators")]
    public class BusinessUnitActuators
    {
        [Key]
        [Column(Order = 1)]
        public int BusinessUnitId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ActuatorId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public virtual BusinessUnit BusinessUnit { get; set; }

        [ForeignKey("ActuatorId")]
        public virtual Actuator Actuator { get; set; }
    }
}