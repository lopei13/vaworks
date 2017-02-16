using Mvc.Mailer;
using VaWorks.Web.Data.Entities;
using VaWorks.Web.ViewModels;

namespace VaWorks.Web.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_Layout";
		}
		
		public virtual MvcMailMessage Welcome(ApplicationUser user)
		{
            ViewData = new System.Web.Mvc.ViewDataDictionary(user);


			return Populate(x =>
			{
				x.Subject = "Welcome To VaWorks";
				x.ViewName = "Welcome";
				x.To.Add(user.Email);
            });
		}

        public virtual MvcMailMessage UserRegistered(ApplicationUser user, string email)
        {
            ViewData = new System.Web.Mvc.ViewDataDictionary(user);
            return Populate(x =>
            {
                x.Subject = "User Registered";
                x.ViewName = "UserRegistered";
                x.To.Add(email);
            });
        }

		public virtual MvcMailMessage Invitation(Invitation invite)
		{
            ViewData = new System.Web.Mvc.ViewDataDictionary(invite);
			return Populate(x =>
			{
				x.Subject = "VaWorks Invitation from VanAire";
				x.ViewName = "Invitation";
				x.To.Add(invite.Email);
                try {
                    x.To.Add(invite.SalesPersonEmail);
                } catch { }
			});
		}
 
		public virtual MvcMailMessage Quote(Quote quote, string email)
		{
            ViewData = new System.Web.Mvc.ViewDataDictionary(quote);

            return Populate(x =>
            {
                x.Subject = "Quote " + quote.QuoteNumber + ":" + quote.Title;
                x.ViewName = "Quote";
                x.To.Add(email);
            });
		}

        public virtual MvcMailMessage QuoteSubmit(Quote quote, string email)
        {
            ViewData = new System.Web.Mvc.ViewDataDictionary(quote);
            
            return Populate(x =>
            {
                x.Subject = "VaWorks Quote Submit " + quote.QuoteNumber;
                x.ViewName = "QuoteSubmit";
                x.To.Add(email);
            });
        }

        public MvcMailMessage InvitationRequest(InvitationRequest invitationRequest, string email)
        {
            ViewData = new System.Web.Mvc.ViewDataDictionary(invitationRequest);
            return Populate(x =>
            {
                x.Subject = "Invitation Request for VaWorks";
                x.ViewName = "InvitationRequest";
                x.To.Add(email);
            });
        }

        public MvcMailMessage QuoteUpdated(Quote quote, string email)
        {
            ViewData = new System.Web.Mvc.ViewDataDictionary(quote);

            return Populate(x =>
            {
                x.Subject = "VaWorks Quote Update " + quote.QuoteNumber;
                x.ViewName = "QuoteUpdated";
                x.To.Add(email);
            });
        }

        public MvcMailMessage SendMessage(string email, string subject, string body)
        {
            ViewBag.Body = body;

            return Populate(x => 
            {
                x.Subject = subject;
                x.ViewName = "SendMessage";
                x.To.Add(email);
            });
        }

        public MvcMailMessage SubmitValveActuatorRequest(ValveActuatorRequestViewModel model, string email)
        {
            ViewData = new System.Web.Mvc.ViewDataDictionary(model);

            return Populate(x => {
                x.Subject = "VaWorks Component Request from " + model.UserName;
                x.ViewName = "SubmitValveActuatorRequest";
                x.To.Add(email);
            });
        }
    }
}