using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace VaWorks.Web.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int? OrganizationId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Title { get; set; }
        
        public string LinkedIn { get; set; }

        public string Facebook { get; set; }

        public string Twitter { get; set; }

        public string Skype { get; set; }

        public string ImageString { get; set; }

        public bool IsSales { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<ShoppingCartItems> ShoppingCart { get; set; }

        public virtual ICollection<ApplicationUser> Contacts { get; set; }

        public virtual ICollection<SystemMessage> Messages { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; }

        public virtual ICollection<Quote> QuotesCreated { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public bool Friend(ApplicationUser contact)
        {
            if(this.Contacts.Where(c => c.Id == contact.Id).FirstOrDefault() == null) {
                this.Contacts.Add(contact);
                if (contact.Contacts.Where(c => c.Id == this.Id).FirstOrDefault() == null) {
                    contact.Contacts.Add(this);
                    return true;
                }
            }

            return false;
        }

        public bool Unfriend(ApplicationUser contact)
        {
            if (this.Contacts.Where(c => c.Id == contact.Id).FirstOrDefault() != null) {
                this.Contacts.Remove(contact);
                if (contact.Contacts.Where(c => c.Id == this.Id).FirstOrDefault() != null) {
                    contact.Contacts.Remove(this);
                    return true;
                }
            }

            return false;
        }

        public void SystemMessage(string message)
        {
            this.Messages.Add(new Entities.SystemMessage() {
                DateSent = DateTimeOffset.Now,
                Message = message
            });
        }
    }

}