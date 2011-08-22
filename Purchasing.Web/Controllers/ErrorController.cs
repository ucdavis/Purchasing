using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Error class
    /// </summary>
    public class ErrorController : ApplicationController
    {
	    
    
        //
        // GET: /Error/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Error 403
        /// View to return when a user is not Authorized.
        /// </summary>
        /// <returns></returns>
        public ActionResult NotAuthorized()
        {
            return View();
        }

        /// <summary>
        /// Error 404
        /// </summary>
        /// <returns></returns>
        public ActionResult FileNotFound()
        {
            return View();
        }

    }
}
