using System;
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
            var addressRepo = Repository.OfType<WorkgroupAddress>();
            var workgroup = Repository.OfType<Workgroup>().Queryable.First();
            var addresses = addressRepo.GetAll();

            var newAddress = new WorkgroupAddress()
                                {
                                    Name = "vendor",
                                    Address = "123 A Street",
                                    City = "city",
                                    State = "CA",
                                    Zip = "90210",
                                    Workgroup = workgroup
                                };

            //addressRepo.EnsurePersistent(newAddress);
            */
            return View();
        }
    }
}