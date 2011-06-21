using System;
using System.Web.Mvc;
using OrAdmin.Core.Enums;
using OrAdmin.Core.Extensions;
using OrAdmin.Core.Enums.App;

namespace OrAdmin.Web.Areas.Home.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Overview()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("login", new { controller = "account", area = "system", ReturnUrl = Request.Path });
            else
                return View();
        }

        public ActionResult Defibrillator()
        {
            this.HttpContext.Session[GlobalProperty.App.Defibrillator.ToString()] = DateTime.Now;
            return new EmptyResult();
        }
    }
}
