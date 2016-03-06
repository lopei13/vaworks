using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.DataAccess.Entities
{
    [Table("Valves")]
    public class Valve
    {
        [Key]
        public int ValveId { get; set; }

        public int InterfaceCode { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(25)]
        public string Manufacturer { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(25)]
        public string Model { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(25)]
        public string Size { get; set; }

        [ForeignKey("InterfaceCode")]
        public virtual ValveInterfaceCode InterfaceCodeEntity { get; set; }
    }

    [Table("ValveInterfaceCodes")]
    public class ValveInterfaceCode
    {
        [Key]
        public int InterfaceCode { get; set; }
    }
}