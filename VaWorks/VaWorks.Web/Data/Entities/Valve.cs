using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.Data.Entities
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

        public virtual ICollection<Organization> Organizations { get; set; }

        public override string ToString()
        {
            return $"{Manufacturer} {Model} {Size}";
        }
    }

    [Table("ValveInterfaceCodes")]
    public class ValveInterfaceCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InterfaceCode { get; set; }
    }
}