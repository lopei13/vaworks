using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VaWorks.Web.Data.Entities;
using VaWorks.Web.ViewModels;
using VaWorks.Web.Data;
using System.Collections.Generic;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using VaWorks.Web.Mailers;

namespace VaWorks.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
    

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationDbContext Database
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = Database.Users.Find(userId);

            if(user == null) {
                return LogOff();
            }
            return View(user);
        }

        public ActionResult Menu()
        {
            if(User.IsInRole("System Administrator")) {
                return View("_AdminMenu");
            }

            return View("_Menu");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplicationUser user, HttpPostedFileBase file)
        {
            if (ModelState.IsValid) {

                var u = Database.Users.Find(user.Id);

                if (file != null) {
                    if (file.ContentLength > 0) {
                        byte[] data = new byte[file.ContentLength];
                        file.InputStream.Read(data, 0, file.ContentLength);

                        u.ImageString = $"data:{file.ContentType};base64,{Convert.ToBase64String(data)}";
                    }
                }

                u.PhoneNumber = user.PhoneNumber;
                u.Email = user.Email;
                u.Name = user.Name;
                //u.UserName = user.UserName;
                u.Facebook = user.Facebook;
                u.Twitter = user.Twitter;
                u.LinkedIn = user.LinkedIn;
                u.Skype = user.Skype;
                u.Title = user.Title;

                //UserManager.Update(u);
                Database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index", user);           
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult RequestAccess()
        {
            return View(new RequestInviteViewModel());
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RequestAccess(RequestInviteViewModel model)
        {
            if (ModelState.IsValid) {

                var invitationRequest = new InvitationRequest() {
                    RequestDate = DateTimeOffset.Now,
                    Company = model.Company,
                    Email = model.Email,
                    Name = model.Name,
                    Status = RequestStatus.New
                };

                Database.InvitationRequests.Add(invitationRequest);

                Database.SaveChanges();

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Database));

                var admins = from role in roleManager.Roles
                             where role.Name == "System Administrator"
                             from u in role.Users
                             select u.UserId;

                IUserMailer mailer = new UserMailer();
                foreach (var admin in admins) {
                    Database.SystemMessages.Add(new SystemMessage() {
                        UserId = admin,
                        DateSent = DateTimeOffset.Now,
                        Message = $"{model.Name} from {model.Company} is requesting access."
                    });

                    // send the admin an email
                    var email = Database.Users.Where(u => u.Id == admin).FirstOrDefault().Email;
                    mailer.InvitationRequest(invitationRequest, email).SendAsync();
                }

                return View("RequestAccessConfirmation");
            }

            return View(model);
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterFromEmail(string email)
        {
            RegisterViewModel vm = new RegisterViewModel() {
                Email = email
            };
            return View("Register", vm);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var invite = Database.Invitations.Where(i => i.Email == model.Email).FirstOrDefault();

                if (invite == null || invite.IsClaimed) {
                    return View("Confirmation", new List<MessageViewModel>() { new MessageViewModel() { AlertType = "Warning", Message = "Invitation code is not valid." } });
                }

                var user = new ApplicationUser {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = $"{model.FirstName} {model.LastName}",
                    PhoneNumber = model.PhoneNumber,
                    IsSales = invite.Type == InvitationType.SalesRepresentive,
                    OrganizationId = invite.OrganizationId
                };
                
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Database));
                    
                    if (invite.Type == InvitationType.SalesRepresentive) {
                        if (!roleManager.RoleExists("Sales")) {
                            roleManager.Create(new IdentityRole("Sales"));
                        }

                        var result2 = UserManager.AddToRole(user.Id, "Sales");
                    }
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    invite.IsClaimed = true;
                    invite.ClaimedDate = DateTimeOffset.Now;
                    invite.ClaimedEmail = model.Email;

                    Database.SystemMessages.Add(new SystemMessage() {
                        UserId = user.Id,
                        DateSent = DateTimeOffset.Now,
                        Message = "Welcome to VAWORKS - The largest database of mounting hardware in the world.  If you have any questions, please do not hesitate to contact VanAire directly."
                    });

                    var org = Database.Organizations.Find(invite.OrganizationId);
                    
                    var admins = from role in roleManager.Roles
                                 where role.Name == "System Administrator"
                                 from u in role.Users
                                 select u.UserId;

                    foreach (var admin in admins) {
                        Database.SystemMessages.Add(new SystemMessage() {
                            UserId = admin,
                            DateSent = DateTimeOffset.Now,
                            Message = $"{user.Name} from {org.Name} has registered."
                        });
                    }

                    // let's see if we can add a sales contact right away
                    var sales = Database.Users.Where(u => u.Email == invite.SalesPersonEmail).FirstOrDefault();
                    if (sales != null) {
                        user.Friend(sales);
                    }

                    Database.SaveChanges();

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public ActionResult GetNewMessageCount()
        {
            var userId = User.Identity.GetUserId();
            var messages = Database.SystemMessages.Where(m => m.UserId == userId).Where(m => !m.IsRead);

            return PartialView("BadgeCount", messages.Count() > 0 ? messages.Count().ToString() : "");
        }

        public JsonResult GetOrganizationDetails(int organizationId)
        {
            var org = from o in Database.Organizations
                      where o.OrganizationId == organizationId
                      select new { o.Name, };

            return Json(org, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MarkMessageAsRead(int messageId)
        {
            var userId = User.Identity.GetUserId();
            var message = Database.SystemMessages
                .Where(m => m.UserMessageId == messageId)
                .Where(m => m.UserId == userId).FirstOrDefault();

            if(message != null) {
                message.IsRead = true;
                message.DateRead = DateTimeOffset.Now;
                Database.SaveChanges();
                return Redirect(Url.RouteUrl(new { controller = "Account", action = "Index" }) + "#messages");
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult DeleteMessage(int messageId)
        {
            var userId = User.Identity.GetUserId();
            var message = Database.SystemMessages
                .Where(m => m.UserMessageId == messageId)
                .Where(m => m.UserId == userId).FirstOrDefault();

            if (message != null) {
                Database.SystemMessages.Remove(message);
                Database.SaveChanges();
                return Redirect(Url.RouteUrl(new { controller = "Account", action = "Index" }) + "#messages");
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult SubmitComponentRequest(ValveActuatorRequestViewModel viewModel, HttpPostedFileBase file)
        {
            var user = Database.Users.Find(User.Identity.GetUserId());

            if (user != null) {
                viewModel.UserName = user.Name;
                viewModel.UserEmail = user.Email;
                viewModel.UserCompany = user.Organization.Name;

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Database));

                var admins = from role in roleManager.Roles
                             where role.Name == "System Administrator"
                             from u in role.Users
                             select u.UserId;

                string message = $"{user.Name} has requested a new {viewModel.ValveOrActuator} be added to the system." + 
                    $"{viewModel.Manufacturer} {viewModel.Model} { viewModel.Size}";
                IUserMailer mailer = new UserMailer();
                foreach (var admin in admins) {
                    Database.SystemMessages.Add(new SystemMessage() {
                        UserId = admin,
                        DateSent = DateTimeOffset.Now,
                        Message = message
                    });

                    // send the email
                    var a = Database.Users.Where(u => u.Id == admin).FirstOrDefault();

                    var msg = mailer.SubmitValveActuatorRequest(viewModel, a.Email);
                    if (file != null) {
                        msg.Attachments.Add(new System.Net.Mail.Attachment(file.InputStream, file.FileName));
                    }
                    msg.Send();
                }
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}