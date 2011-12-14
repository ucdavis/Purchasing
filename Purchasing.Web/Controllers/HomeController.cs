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
        /// <summary>
        /// Landing Page welcoming Users to the PrePurchasing System
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {          
            return View();
        }

        /// <summary>
        /// Authorized user's landing page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Landing()
        {
            return View();
        }

        public ActionResult Dev()
        {
            ViewBag.Users = Repository.OfType<User>().GetAll();

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