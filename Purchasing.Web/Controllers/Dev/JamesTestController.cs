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
        private readonly IRepositoryWithTypedId<ColumnPreferences, string> _columnPreferences;


        public JamesTestController(IOrderAccessService orderAccessService, IRepositoryWithTypedId<ColumnPreferences, string> columnPreferences )
        {
            _orderAccessService = orderAccessService;
            _columnPreferences = columnPreferences;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusFilter"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="showAll">Matches AllActive in GetListOfOrders</param>
        /// <param name="showCompleted">Matches All in GetListOfOrders</param>
        /// <param name="showOwned"></param>
        /// <returns></returns>
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

            //var x = Repository.OfType<OrderTracking>().Queryable.Where(a => a.User.Id == CurrentUser.Identity.Name).GroupBy(a => a.Order).
             


        }

        public ActionResult Index2(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false)
        {
            if (statusFilter == null)
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
            viewModel.ColumnPreferences = _columnPreferences.GetNullableById(CurrentUser.Identity.Name) ??
                                          new ColumnPreferences(CurrentUser.Identity.Name);

            return View(viewModel);

            //var x = Repository.OfType<OrderTracking>().Queryable.Where(a => a.User.Id == CurrentUser.Identity.Name).GroupBy(a => a.Order).



        }

        /// <summary>
        /// This returns a list of orders created by the current user than have not been acted on X number of days.
        /// It Does this by querying the orderTracking to find the last date acted on and filter those that have been acted on
        /// The it checks all those orders to see which ones have a status that is complete and filter out those. 
        /// </summary>
        /// <param name="id">The number of days when an order was lasted acted on</param>
        /// <returns></returns>
        public ActionResult Test(int id)
        {
            //TODO: Might be better to have a trigger in the database update the order to have the last acted upon date
            var orderIds = GetListOfOrderIds(id);
            
            var results = Repository.OfType<Order>().Queryable.Where(a => orderIds.Contains(a.Id) && !a.OrderTrackings.Where(b => b.StatusCode.IsComplete).Any()).Distinct().ToList();
            //var results = Repository.OfType<Order>().Queryable.Where(a => a.CreatedBy.Id == CurrentUser.Identity.Name && !a.OrderTrackings.Where(b => !b.StatusCode.IsComplete && b.DateCreated >= dt).Any()).Distinct().ToList();

            var viewModel = FilteredOrderListModel.Create(Repository, results);


            return View(viewModel);
        }

        private List<int> GetListOfOrderIds(int id)
        {
            var dt = DateTime.Now.AddDays(-id).Date;

            var orderIds = (Repository.OfType<OrderTracking>().Queryable
                .Where(a => a.Order.CreatedBy.Id == CurrentUser.Identity.Name)
                .GroupBy(s => s.Order.Id).Select(g => new {id = g.Key, LastDateActedOn = g.Max(s => s.DateCreated)}).ToList().
                Where(xy => xy.LastDateActedOn <= dt)).Select(cx => cx.id).ToList();
            return orderIds;
        }

        public ActionResult LandingPage()
        {
            var landingPageViewModel = new LandingPageViewModel();
            landingPageViewModel.YourOpenRequestCount = _orderAccessService.GetListofOrders(owned: true).Count();
            landingPageViewModel.YourNotActedOnCount = GetListOfOrderIds(7).Count();
            landingPageViewModel.OrdersPendingYourActionCount = _orderAccessService.GetListofOrders().Count();
            landingPageViewModel.ColumnPreferences = GetLandingPageColumnPreferences();
            
            var codes = Repository.OfType<OrderStatusCode>().Queryable.Where(a => a.ShowInFilterList).OrderBy(a => a.Level).ToList();


            var oldOrders = _orderAccessService.GetListofOrders().OrderBy(a => a.DateCreated).Take(5).ToList();
            var newestOrders =
                _orderAccessService.GetListofOrders().OrderByDescending(a => a.DateCreated).Take(5).ToList();
            landingPageViewModel.OldestFOLM = FilteredOrderListModel.Create(Repository, oldOrders, codes);
            landingPageViewModel.OldestFOLM.ColumnPreferences = landingPageViewModel.ColumnPreferences;

            landingPageViewModel.NewestFOLM = FilteredOrderListModel.Create(Repository, newestOrders, codes);
            landingPageViewModel.NewestFOLM.ColumnPreferences = landingPageViewModel.ColumnPreferences;
            var lastFive =
                Repository.OfType<OrderTracking>().Queryable.Where(a => a.User.Id == CurrentUser.Identity.Name).
                    OrderByDescending(a => a.DateCreated).Select(a => a.Order).Take(5).ToList();

            landingPageViewModel.LastFiveFOLM = FilteredOrderListModel.Create(Repository, lastFive, codes);
            landingPageViewModel.LastFiveFOLM.ColumnPreferences = landingPageViewModel.ColumnPreferences;

            return View(landingPageViewModel);
        }

        // TODO: Account for cancelled orders once processing of cancelled orders is complete
        public ActionResult RequesterAmmountSummary()
        {

            var completed = Repository.OfType<Approval>().Queryable.Where(
                    a =>
                    ((a.User != null && a.User.Id == CurrentUser.Identity.Name) ||
                     (a.SecondaryUser != null && a.SecondaryUser.Id == CurrentUser.Identity.Name)) &&
                    a.StatusCode.Id == OrderStatusCode.Codes.Approver).Select(b => b.Order).ToList().Where(c => c.OrderTrackings.Where(d => d.StatusCode.IsComplete).Any())
                .Select(e => new { e.CreatedBy, Pending = 0, Completed = e.TotalFromDb }).ToList()
                .GroupBy(f => f.CreatedBy).Select(g => new { id = g.Key, Pending = 0, Completed = g.Sum(s => s.Completed) }).ToList();


            var open = Repository.OfType<Approval>().Queryable.Where(
                    a =>
                    ((a.User != null && a.User.Id == CurrentUser.Identity.Name) ||
                     (a.SecondaryUser != null && a.SecondaryUser.Id == CurrentUser.Identity.Name)) &&
                    a.StatusCode.Id == OrderStatusCode.Codes.Approver).Select(b => b.Order).ToList().Where(c => !c.OrderTrackings.Where(d => d.StatusCode.IsComplete).Any())
                .Select(e => new { e.CreatedBy, Pending = 0, Completed = e.TotalFromDb }).ToList()
                .GroupBy(f => f.CreatedBy).Select(g => new { id = g.Key, Pending = 0, Completed = g.Sum(s => s.Completed) }).ToList();

            var temp = Repository.OfType<Approval>().Queryable.Where(
                a =>
                ((a.User != null && a.User.Id == CurrentUser.Identity.Name) ||
                 (a.SecondaryUser != null && a.SecondaryUser.Id == CurrentUser.Identity.Name)) &&
                a.StatusCode.Id == OrderStatusCode.Codes.Approver).Select(b => b.Order).ToList().Where(c => c.OrderTrackings.Where(d => d.StatusCode.IsComplete).Any())
                .Select(e => new { e.Id, e.CreatedBy, Pending = 0, Completed = e.TotalFromDb }).ToList();

            var temp2 = Repository.OfType<Approval>().Queryable.Where(
                a =>
                ((a.User != null && a.User.Id == CurrentUser.Identity.Name) ||
                 (a.SecondaryUser != null && a.SecondaryUser.Id == CurrentUser.Identity.Name)) &&
                a.StatusCode.Id == OrderStatusCode.Codes.Approver).Select(b => b.Order).ToList().Where(c => !c.OrderTrackings.Where(d => d.StatusCode.IsComplete).Any())
                .Select(e => new { e.Id, e.CreatedBy, Pending = 0, Completed = e.TotalFromDb }).ToList();

            // var open = orders.Where(a => !a.OrderTrackings.Where(b => b.StatusCode.IsComplete).Any());

            return View();
        }

        public ActionResult LandingPage2()
        {
            return LandingPage();
        }

        public static ColumnPreferences GetLandingPageColumnPreferences()
        {
            var rtValue = new ColumnPreferences();
            //These are the default columns
            rtValue.ShowWorkgroup = false;
            rtValue.ShowVendor = false;
            rtValue.ShowCreatedBy = true;
            rtValue.ShowCreatedDate = true;
            rtValue.ShowStatus = false;
            rtValue.ShowNeededDate = false;
            rtValue.ShowDaysNotActedOn = true;
            rtValue.ShowAccountManager = false;

            rtValue.ShowRequestNumber = true;

            return rtValue;

        }
    }

    public class LandingPageViewModel
    {
        public int YourOpenRequestCount { get; set; }
        public int YourNotActedOnCount { get; set; }
        public int OrdersPendingYourActionCount { get; set; }
        public ColumnPreferences ColumnPreferences { get; set; }
        public List<RequestedHistory> RequestedHistories { get; set; }
        public FilteredOrderListModel OldestFOLM { get; set; }
        public FilteredOrderListModel NewestFOLM { get; set; }
        public FilteredOrderListModel LastFiveFOLM { get; set; }
        
    }

    public class RequestedHistory
    {
        public string Name { get; set; }
        public decimal PendingTotal { get; set; }
        public decimal CompletedTotal { get; set; }
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
        public ColumnPreferences ColumnPreferences { get; set; }

        public static FilteredOrderListModel Create(IRepository repository, IList<Order> orders, List<OrderStatusCode> orderStatusCodes = null )
        {
            Check.Require(repository != null, "Repository must be supplied");

            var viewModel = new FilteredOrderListModel
            {
                Orders = orders
            };
            if (orderStatusCodes == null)
            {
                viewModel.OrderStatusCodes =
                    repository.OfType<OrderStatusCode>().Queryable.Where(a => a.ShowInFilterList).OrderBy(a => a.Level).
                        ToList();
            }
            else
            {
                viewModel.OrderStatusCodes = orderStatusCodes;
            }
            viewModel.CheckedOrderStatusCodes = new List<string>();

            return viewModel;
        }
    }
	
}
