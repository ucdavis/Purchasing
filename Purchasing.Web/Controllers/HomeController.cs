using System.Linq;
using System.Web.Mvc;
using UCDArch.Web.Attributes;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Controllers
{
    [HandleTransactionsManually] //Don't create transactions for home controller methods
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
