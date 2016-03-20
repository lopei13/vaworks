using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.Data.Entities
{
    [Table("ShoppingCartItems")]
    public class ShoppingCartItems
    {
        [Key]
        public int ShoppingCartItemId { get; set; }

        public string UserId { get; set; }

        public int? KitId { get; set; }

        public int? ValveId { get; set; }

        public int? ActuatorId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("KitId")]
        public virtual Kit Kit { get; set; }

        [ForeignKey("ValveId")]
        public virtual Valve Valve { get; set; }

        [ForeignKey("ActuatorId")]
        public virtual Actuator Actuator { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public override string ToString()
        {
            if (Actuator != null && Valve != null) {
                return $"KIT FOR {Actuator.ToString()} TO {Valve.ToString()}";
            } else {
                return "Invalid item";
            }
        }
    }
}