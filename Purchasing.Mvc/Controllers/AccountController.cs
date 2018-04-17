using System.Web.Mvc;
using System.Web.Security;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Helpers;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Account class.
    /// </summary>
    public class AccountController : Controller 
    {
        public string Message
        {
            set { TempData["Message"] = value; }
        }
        public ActionResult LogOn(string returnUrl)
        {
            string resultUrl = CasHelper.Login(); //Do the CAS Login

            if (resultUrl != null)
            {
                 return Redirect(resultUrl);
            }

            TempData["URL"] = returnUrl;

            return View();

        }

        public ActionResult LogOut()
        {
            return Redirect(CasHelper.Logout());
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