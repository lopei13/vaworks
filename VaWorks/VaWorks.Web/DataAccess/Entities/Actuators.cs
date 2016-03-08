using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.DataAccess.Entities
{
    [Table("Actuators")]
    public class Actuator
    {
        [Key]
        public int ActuatorId { get; set; }

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
        public virtual ActuatorInterfaceCode InterfaceCodeEntity { get; set; }
    }

    [Table("ActuatorInterfaceCodes")]
    public class ActuatorInterfaceCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterfaceCode { get; set; }
    }
}