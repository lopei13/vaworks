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
        public int BusinessUnitId { get; set; }

        [Display(Name = "Parent Business Unit")]
        public int? ParentBusinessUnitId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        [ForeignKey("ParentBusinessUnitId")]
        public virtual BusinessUnit ParentBusinessUnit { get; set; }

        public virtual ICollection<BusinessUnit> Children { get; set; }
    }
}