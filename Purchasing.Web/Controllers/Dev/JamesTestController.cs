using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
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

        public ActionResult Index(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showall = false)
        {
            if(statusFilter==null)
            {
                statusFilter = new string[0];
            }
            var filters = statusFilter.ToList();

            var orders = _orderAccessService.GetListofOrders();
            var viewModel = FilteredOrderListModel.Create(Repository, orders);
            viewModel.CheckedOrderStatusCodes = filters;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.Showall = showall;

            return View(viewModel);
        }

    }

    public class FilteredOrderListModel
    {
        public List<OrderStatusCode> OrderStatusCodes { get; set; }
        public IList<Order> Orders { get; set; }
        public List<string> CheckedOrderStatusCodes { get; set; }
        public bool Showall { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static FilteredOrderListModel Create(IRepository repository, IList<Order> orders)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new FilteredOrderListModel
            {
                Orders = orders
            };
            viewModel.OrderStatusCodes = repository.OfType<OrderStatusCode>().Queryable.ToList();
            viewModel.CheckedOrderStatusCodes = new List<string>();

            return viewModel;
        }
    }
	
}
