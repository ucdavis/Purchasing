using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Web.Attributes;

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
            var user = Repository.OfType<User>().Queryable.Where(x => x.Id == "postit").Single();

            return View();
        }
    }
}
