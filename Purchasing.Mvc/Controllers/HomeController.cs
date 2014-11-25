using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Web.Controller;

namespace Purchasing.Mvc.Controllers
{
    public class HomeController : SuperController
    {
        public ActionResult Index()
        {
            var a = Repository.OfType<Approval>().Queryable.First();
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}