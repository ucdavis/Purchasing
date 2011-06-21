using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Xml;
using OrAdmin.Core.Extensions;
using OrAdmin.Core.Settings;
using OrAdmin.Entities.App;
using OrAdmin.Repositories.App;
using OrAdmin.Web.Areas.System.Models.Account;

namespace OrAdmin.Web.Areas.System.Controllers
{
    [HandleError]
    public class AccountController : BaseController
    {
        public AccountViewModel.IFormsAuthenticationService FormsService { get; set; }
        public AccountViewModel.IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new AccountViewModel.FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountViewModel.AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogIn()
        {
            // If CAS ticket is present...
            string ticket = Request.QueryString["ticket"];
            if (!String.IsNullOrEmpty(ticket))
            {
                // Second time (back from CAS) there is a ticket= to validate
                string service = Request.QueryString["return"] != null ?
                    Request.Url.GetLeftPart(UriPartial.Path) + "?return=" + Request.QueryString["return"] :
                    Request.Url.GetLeftPart(UriPartial.Path);

                string validateurl = String.Format("{0}serviceValidate?ticket={1}&service={2}", GlobalSettings.CASHost, ticket, service);

                StreamReader Reader = new StreamReader(new WebClient().OpenRead(validateurl));
                string resp = Reader.ReadToEnd();
                // I like to have the text in memory for debugging rather than parsing the stream

                // Some boilerplate to set up the parse.
                NameTable nt = new NameTable();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                XmlTextReader reader = new XmlTextReader(resp, XmlNodeType.Element, context);

                string netid = null;

                // A very dumb use of XML. Just scan for the "user". If it isn't there, its an error.
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string tag = reader.LocalName;
                        if (tag == "user")
                            netid = reader.ReadString();
                    }
                }
                // If you want to parse the proxy chain, just add the logic above
                reader.Close();

                // If there was a problem, leave the message on the screen. Otherwise, return to original page.
                if (netid == null)
                    ModelState.AddModelError("", "CAS returned to this application, but then refused to validate your identity.");
                else
                {
                    // Sign user in
                    FormsService.SignIn(netid, false);

                    // Redirect or home?
                    if (!String.IsNullOrEmpty(Request.QueryString["return"]))
                        return Redirect(Encoding.UTF8.GetString(Server.UrlTokenDecode(Request.QueryString["return"])));
                    else
                        return RedirectToAction("index", new { controller = "home", area = "home" });
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string returnUrl)
        {
            // CAS Auth
            string service = String.IsNullOrEmpty(returnUrl) ?
                 Request.Url.GetLeftPart(UriPartial.Path) :
                 Request.Url.GetLeftPart(UriPartial.Path) + "?return=" + Server.UrlTokenEncode(Encoding.UTF8.GetBytes(returnUrl));

            // First time through there is no ticket=, so redirect to CAS login
            if (String.IsNullOrEmpty(Request.QueryString["ticket"]))
                return new RedirectResult(String.Format("{0}login?service={1}", GlobalSettings.CASHost, service));

            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult PublicLogIn()
        {
            return View();
        }

        [HttpPost]
        //AccountViewModel.LogInModel model 
        public ActionResult PublicLogIn(string UserName, string Password, string returnUrl, bool RememberMe)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(UserName, Password))
                {
                    FormsService.SignIn(UserName, RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("index", "home");
                }
                else
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsService.SignOut();
                return RedirectToAction("logout");
            }

            return View();
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(AccountViewModel.RegisterModel model, string returnUrl)
        {
            // UC Davis registration
            if (ModelState.IsValid)
            {
                // TODO: LDAP lookup here to grab name, email, etc...
                // Attempt to register the user using generic password
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.Kerberos, "generatedhashorguid", "email@from.ldap");

                if (createStatus == MembershipCreateStatus.Success)
                {
                    // Do not automatically sign-in user... they have not authenticated yet and therefore could "hijack" someone elses kerberos login ID
                    // FormsService.SignIn(kerberos, false /* createPersistentCookie */);

                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("index", "home");
                }
                else
                    ModelState.AddModelError("", AccountViewModel.AccountValidation.ErrorCodeToString(createStatus));
            }

            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        // 
        public ActionResult ChangePassword(string OldPassword, string NewPassword)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, OldPassword, NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult Profile()
        {
            ProfileRepository repo = new ProfileRepository();
            return View(repo.GetProfile(User.Identity.Name));
        }

        [Authorize]
        [HttpPost]
        public ActionResult Profile([Bind(Exclude = "Id")]Profile profile, FormCollection collection, int? Id, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ProfileRepository repo = new ProfileRepository();

                if (User.HasProfile())
                {
                    // Update profile
                    Profile profileToUpdate = repo.GetProfile(Convert.ToInt32(Id));
                    profileToUpdate.LastUpdated = DateTime.Now;
                    UpdateModel(profileToUpdate, collection);
                    repo.Save();
                }
                else
                {
                    // Insert profile
                    profile.UserName = User.Identity.Name;
                    profile.LastUpdated = DateTime.Now;
                    repo.InsertProfile(profile);
                    repo.Save();
                }

                // Success!
                TempData[OrAdmin.Core.Enums.App.GlobalProperty.Message.SuccessMessage.ToString()] = "Profile successfully updated!";

                // Redirect or return
                if (String.IsNullOrEmpty(returnUrl))
                    return View(User.Profile());
                else
                    return Redirect(Server.UrlDecode(returnUrl));
            }
            else
                return View(profile);
        }

    }
}
