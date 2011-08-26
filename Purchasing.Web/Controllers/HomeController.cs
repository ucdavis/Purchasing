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
            /*
            var vendorRepo = Repository.OfType<WorkgroupVendor>();
            var workgroup = Repository.OfType<Workgroup>().Queryable.First();
            var vendors = vendorRepo.GetAll();

            var newVendor = new WorkgroupVendor
                                {
                                    Name = "vendor",
                                    Line1 = "addr1",
                                    City = "city",
                                    State = "CA",
                                    Zip = "90210",
                                    Workgroup = workgroup
                                };

            //vendorRepo.EnsurePersistent(newVendor);
            */

            return View();
        }
    }
}