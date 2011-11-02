using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public ActionResult Index(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false)
        {
            if(statusFilter==null)
            {
                statusFilter = new string[0];
            }
            
            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, list, startDate, endDate);
            var viewModel = FilteredOrderListModel.Create(Repository, orders);
            viewModel.CheckedOrderStatusCodes = filters;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.ShowAll = showAll;
            viewModel.ShowCompleted = showCompleted;
            viewModel.ShowOwned = showOwned;

            return View(viewModel);
        }

        public ActionResult LandingPage()
        {
            //var orders = Repository.OfType<OrderTracking>().Queryable.Where(a => a.User.Id == CurrentUser.Identity.Name);
            //var q = Repository.OfType<OrderTracking>().Queryable.GroupBy(t => t.Order).Select(g => new { id = g.Key, uid = (from t2 in g select t2.DateCreated).Max() });
            var result = from a in Repository.OfType<OrderTracking>().Queryable group a by a.Order into x select new { Order = x.Key, LastUpdate = x.Max(a => a.DateCreated) };
            return null;
        }

    }

    public class FilteredOrderListModel
    {
        public List<OrderStatusCode> OrderStatusCodes { get; set; }
        public IList<Order> Orders { get; set; }
        public List<string> CheckedOrderStatusCodes { get; set; }
        public bool ShowAll { get; set; }
        public bool ShowCompleted { get; set; }
        [Display(Name = "Created After")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Created Before")]
        public DateTime? EndDate { get; set; }
        public bool ShowOwned { get; set; }

        public static FilteredOrderListModel Create(IRepository repository, IList<Order> orders)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new FilteredOrderListModel
            {
                Orders = orders
            };
            viewModel.OrderStatusCodes =
                repository.OfType<OrderStatusCode>().Queryable.Where(a => a.ShowInFilterList).OrderBy(a => a.Level).
                    ToList();
            viewModel.CheckedOrderStatusCodes = new List<string>();

            return viewModel;
        }
    }
	
}
