﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.Data.Entities
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

        public virtual ICollection<Organization> Organizations { get; set; }

        public override string ToString()
        {
            return $"{Manufacturer} {Model} {Size}";
        }
    }

    [Table("ActuatorInterfaceCodes")]
    public class ActuatorInterfaceCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InterfaceCode { get; set; }
    }
}