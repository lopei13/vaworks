using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.Data.Entities
{
    [Table("Quotes")]
    public class Quote
    {
        [Key]   
        public int QuoteId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int QuoteNumber { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string SalesPerson { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; }

        [Required]
        public DateTimeOffset ModifiedDate { get; set; }

        [Required]
        public DateTimeOffset OrderDate { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        public bool IsSent { get; set; }

        public bool IsOrder { get; set; }
    }

    [Table("QuoteItems")]
    public class QuoteItems
    {
        [Key]
        public int QuoteItemId { get; set; }

        [MaxLength(100)]
        public string KitNumber { get; set; }

        [MaxLength(100)]
        public string Valve { get; set; }

        [MaxLength(100)]
        public string Actuator { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        public double PriceEach { get; set; }

        public double Discount { get; set; }

        public double TotalPrice { get; set; }
    }

    [Table("Discounts")]
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        public int OrganizationId { get; set; }

        public int Quantity { get; set; }

        [Range(0, 100)]
        [Display(Name = "Discount")]
        public double DiscountPercentage { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
    }
}