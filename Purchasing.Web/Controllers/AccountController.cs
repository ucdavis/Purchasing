using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using Purchasing.Core.Domain;
using UCDArch.Web.Authentication;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Account class
    /// </summary>
    public class AccountController : Controller
    {
        public string Message
        {
            set { TempData["Message"] = value; }
        }
        public ActionResult LogOn(string returnUrl)
        {
            string resultUrl = CASHelper.Login(); //Do the CAS Login

            if (resultUrl != null)
            {
                /*
                    this has been causing the "server cannot set status after HTTP headers have been sent elmah errors
                    suggested fix from : http://stackoverflow.com/questions/2383169/server-cannot-set-status-after-http-headers-have-been-sent-iis7-5
                */
                 //return Redirect(resultUrl);
                Response.Redirect(resultUrl, true);
            }

            TempData["URL"] = returnUrl;

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("https://cas.ucdavis.edu/cas/logout");
        }

        /// <summary>
        /// Emulate a specific user, for Emulation Users only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Codes.EmulationUser)]
        public RedirectToRouteResult Emulate(string id /* Login ID*/)
        {
            if (!string.IsNullOrEmpty(id))
            {
                //Message = "Emulating " + id;
                Message = string.Format("Emulating {0}.  To exit emulation use /Account/EndEmulation", id);
                FormsAuthentication.RedirectFromLoginPage(id, false);
            }
            else
            {
                Message = "Login ID not provided.  Use /Emulate/login";
            }
            
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Just a signout, without the hassle of signing out of CAS.  Ends emulated credentials.
        /// </summary>
        /// <returns></returns>
        public RedirectToRouteResult EndEmulate()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}