using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaWorks.Web.Data.Entities
{
    [Table("Organizations")]
    public class Organization
    {
        [Key]
        [Display(Name = "Organization")]
        public int OrganizationId { get; set; }

        [Display(Name = "Parent Organization")]
        public int? ParentId { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        [ForeignKey("ParentId")]
        public virtual Organization ParentOrganization { get; set; }

        public virtual ICollection<Organization> Children { get; set; }

        public virtual ICollection<Kit> Kits { get; set; }

        public virtual ICollection<Valve> Valves { get; set; }

        public virtual ICollection<Actuator> Actuators { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<Discount> Discounts { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; }

        public IEnumerable<Quote> GetAllQuotes()
        {
            List<Quote> quotes = new List<Quote>();

            Stack<Organization> stack = new Stack<Organization>();
            stack.Push(this);

            while (stack.Any()) {
                var org = stack.Pop();
                foreach (var q in org.Quotes) {
                    quotes.Add(q);
                }

                foreach (var o in org.Children) {
                    stack.Push(o);
                }
            }

            return quotes.OrderBy(q => q.QuoteNumber);
        }
    }
}