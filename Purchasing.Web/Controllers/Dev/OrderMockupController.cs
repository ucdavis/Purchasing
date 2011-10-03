

using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the OrderMockup class
    /// </summary>
    public class OrderMockupController : ApplicationController
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderMockupController(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        //
        // GET: /OrderMockup/
        public ActionResult Index()
        {
            return View();
        }


        public new ActionResult Request()
        {
            ViewBag.Accounts = Repository.OfType<WorkgroupAccount>().Queryable.Select(x=>x.Account).ToList();
            ViewBag.Vendors = Repository.OfType<WorkgroupVendor>().GetAll();
            ViewBag.Approvers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.Approver).Select(
                    x => x.User).ToList();
            ViewBag.AccountManagers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.AccountManager).Select(
                    x => x.User).ToList();

            return View();
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddVendor()
        {
            return Json(new {id = new Random().Next(100)});
        }
        
        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddAddress()
        {
            return Json(new { id = new Random().Next(100) });
        }
    }
}
