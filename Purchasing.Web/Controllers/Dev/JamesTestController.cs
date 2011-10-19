using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers.Dev
{
    /// <summary>
    /// Controller for the JamesTest class
    /// </summary>
    public class JamesTestController : ApplicationController
    {
	    private readonly IOrderAccessService _orderAccessService;

        public JamesTestController(IOrderAccessService orderAccessService)
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
