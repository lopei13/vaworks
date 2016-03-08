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
        [Display(Name = "Kit Number")]
        [MinLength(1)]
        [MaxLength(30)]
        public string KitNumber { get; set; }

        public int ValveInterfaceCode { get; set; }

        public int ActuatorInterfaceCode { get; set; }

        public int KitMaterialId { get; set; }

        public int KitOptionId { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [ForeignKey("KitMaterialId")]
        public virtual KitMaterial Material { get; set; }

        [ForeignKey("KitOptionId")]
        public virtual KitOption Option { get; set; }

        [ForeignKey("ValveInterfaceCode")]
        public virtual ValveInterfaceCode ValuveIntefaceCodeEntity { get; set; }

        [ForeignKey("ActuatorInterfaceCode")]
        public virtual ActuatorInterfaceCode ActuatorInterfaceCodeEntity { get; set; }

        public virtual ICollection<Organization> Organizations { get; set; }
    }

    [Table("KitConfigurations")]
    public class KitConfiguration
    {
        [Key]
        public int KitConfigurationId { get; set; }

        public int KitMaterialId { get; set; }

        public int KitOptionId { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [ForeignKey("KitMaterialId")]
        public virtual KitMaterial Material { get; set; }

        [ForeignKey("KitOptionId")]
        public virtual KitOption Option { get; set; }
    }

    [Table("KitMaterials")]
    public class KitMaterial
    {
        [Key]
        public int KitMaterialId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        public string Description { get; set; }

        public byte[] FileData { get; set; }

        public string ContentType { get; set; }
    }

    [Table("KitOptions")]
    public class KitOption
    {
        [Key]
        public int KitOptionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
    }
}