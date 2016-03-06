using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.DataAccess.Entities
{
    [Table("Kits")]
    public class Kit
    {
        [Key]
        public int KitId { get; set; }

        [Required]
        [Display(Name = "Base Kit Number")]
        public string BaseNumber { get; set; }

        public int ValveInterfaceCode { get; set; }

        public int ActuatorInterfaceCode { get; set; }

        [ForeignKey("ValveInterfaceCode")]
        public virtual ValveInterfaceCode ValuveIntefaceCodeEntity { get; set; }

        [ForeignKey("ActuatorInterfaceCode")]
        public virtual ActuatorInterfaceCode ActuatorInterfaceCodeEntity { get; set; }
    }
}