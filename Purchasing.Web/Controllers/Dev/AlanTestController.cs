using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the AlanTest class
    /// </summary>
    public class AlanTestController : ApplicationController
    {
        private readonly IOrderAccessService _orderAccessService;

        public AlanTestController(IOrderAccessService orderAccessService)
        {
            _orderAccessService = orderAccessService;
        }

        public ActionResult Index()
        {
            var orders = _orderAccessService.GetListofOrders();

            return View(orders);
        }

    }
}
