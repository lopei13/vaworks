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

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        [ForeignKey("ParentId")]
        public virtual Organization ParentOrganization { get; set; }

        public virtual ICollection<Organization> Children { get; set; }

        public virtual ICollection<Kit> Kits { get; set; }

        public virtual ICollection<Valve> Valves { get; set; }

        public virtual ICollection<Actuator> Actuators { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<Discount> Discounts { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        //public IEnumerable<Quote> GetAllQuotes()
        //{
        //    HashSet<Quote> quotes = new HashSet<Quote>();

        //    Stack<Organization> stack = new Stack<Organization>();
        //    stack.Push(this);

        //    while (stack.Any()) {
        //        var org = stack.Pop();
        //        foreach (var q in org.Quotes.Where(q => q.IsSent)) {
        //            quotes.Add(q);
        //        }

        //        foreach (var o in org.Children) {
        //            stack.Push(o);
        //        }
        //    }

        //    return quotes.OrderByDescending(q => q.QuoteNumber);
        //}

        public string GetCompanyName()
        {
            string name = "";

            Organization o = this;

            while (o != null) {
                name += name + o.Name + " ";
                o = o.ParentOrganization;
            }

            return name;
        }

        public IEnumerable<Organization> GetAllOrganizations()
        {
            yield return this;
            foreach(var c in Children) {
                foreach(var o in c.GetAllOrganizations()) {
                    yield return o;
                }
            }
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            foreach (var o in GetAllOrganizations()) {
                foreach (var u in o.Users) {
                    yield return u;
                }
            }

        }

        public IEnumerable<Quote> GetAllQuotes()
        {
            foreach(var u in GetAllUsers()) {
                foreach(var q in u.Quotes) {
                    yield return q;
                }
            }
        }
    }
}