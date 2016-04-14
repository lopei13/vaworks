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
        public Quote()
        {
            Items = new List<QuoteItem>();
        }

        [Key]   
        public int QuoteId { get; set; }

        [Required]
        public string CreatedById { get; set; }

        public string CustomerId { get; set; }

        public int OrganizationId { get; set; }

        [Required]
        public int QuoteNumber { get; set; }

        [StringLength(5)]
        public string Revision { get; set; }

        [Required]
        public string CreatedByName { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

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

        [DataType(DataType.Currency)]
        public double Total { get; set; }

        public bool IsSent { get; set; }

        public bool IsOrder { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; }

        public virtual IList<QuoteItem> Items { get; set; }
    }

    [Table("QuoteItems")]
    public class QuoteItem
    {
        [Key]
        public int QuoteItemId { get; set; }

        public  int QuoteId { get; set; }

        [MaxLength(100)]
        public string KitNumber { get; set; }

        [MaxLength(100)]
        public string Valve { get; set; }

        [MaxLength(100)]
        public string Actuator { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public double PriceEach { get; set; }

        public double Discount { get; set; }

        [DataType(DataType.Currency)]
        public double TotalPrice { get; set; }

        [ForeignKey("QuoteId")]
        public virtual Quote Quote { get; set; }
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

    [Table("QuoteNumber")]
    public class QuoteNumber
    {
        [Key]
        public int Id { get; set; }

        public int Number { get; set; }
    }
}