using Mvc.Mailer;
using VaWorks.Web.Data.Entities;
using VaWorks.Web.ViewModels;

namespace VaWorks.Web.Mailers
{
    public interface IUserMailer
    {
        MvcMailMessage Welcome(ApplicationUser user);
        MvcMailMessage UserRegistered(ApplicationUser user, string email);
        MvcMailMessage Invitation(Invitation invite);
        MvcMailMessage InvitationRequest(InvitationRequest invitationRequest, string email);
        MvcMailMessage Quote(Quote quote, string email);
        MvcMailMessage QuoteSubmit(Quote quote, string eamil);
        MvcMailMessage QuoteUpdated(Quote quote, string email);
        MvcMailMessage SendMessage(string email, string subject, string body);
        MvcMailMessage SubmitValveActuatorRequest(ValveActuatorRequestViewModel model, string email);
    }
}