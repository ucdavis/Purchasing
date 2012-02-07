using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the JamesTest class
    /// </summary>
    public class JamesTestController : ApplicationController
    {
        private readonly IOrderService _orderAccessService;
        private readonly IRepositoryWithTypedId<ColumnPreferences, string> _columnPreferences;


        public JamesTestController(IOrderService orderAccessService, IRepositoryWithTypedId<ColumnPreferences, string> columnPreferences)
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
        public ActionResult Index(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideCreatedByYou = false)
        {
            if(statusFilter==null)
            {
                statusFilter = new string[0];
            }
            
            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, hideCreatedByYou, list, startDate, endDate);
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

        public ActionResult Index2(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideCreatedByYou = false)
        {
            if (statusFilter == null)
            {
                statusFilter = new string[0];
            }

            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, hideCreatedByYou, list, startDate, endDate);
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

            ViewBag.ShowRequestorSummaryTotals = Repository.OfType<Approval>().Queryable
                .Any(a => ((a.User != null && a.User.Id == CurrentUser.Identity.Name) ||
                    (a.SecondaryUser != null && a.SecondaryUser.Id == CurrentUser.Identity.Name)) &&
                    a.StatusCode.Id == OrderStatusCode.Codes.Approver);

            landingPageViewModel.RequesterTotals = new List<RequesterTotals>();
            if(ViewBag.ShowRequestorSummaryTotals)
            {
                landingPageViewModel.RequesterTotals = GetRequesterTotals("M"); //Default to Month. If you change this you have to change the view too.
            }

            return View(landingPageViewModel);
        }

        /// <summary>
        /// Ajax call
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public JsonNetResult RequestAmountSummaryCall (string filter)
        {
           return new JsonNetResult(GetRequesterTotals(filter));
        }

        private List<RequesterTotals> GetRequesterTotals(string filter)
        {
            var ordersPreFilter = Repository.OfType<Approval>().Queryable.Where(
                a =>
                ((a.User != null && a.User.Id == CurrentUser.Identity.Name) ||
                 (a.SecondaryUser != null && a.SecondaryUser.Id == CurrentUser.Identity.Name)) &&
                a.StatusCode.Id == OrderStatusCode.Codes.Approver).Select(b => b.Order);

            List<Order> orders;
            var localDate = DateTime.Now.Date;
            switch(filter)
            {
                case "A":
                    orders = ordersPreFilter.ToList();
                    break;
                case "W":
                    localDate = localDate.AddDays(-7);
                    orders = ordersPreFilter.Where(a => a.DateCreated >= localDate).ToList();
                    break;
                case "M":
                    localDate = localDate.AddMonths(-1);
                    orders = ordersPreFilter.Where(a => a.DateCreated >= localDate).ToList();
                    break;
                case "Y":
                    localDate = localDate.AddYears(-1);
                    orders = ordersPreFilter.Where(a => a.DateCreated >= localDate).ToList();
                    break;
                default:
                    localDate = localDate.AddMonths(-1);
                    orders = ordersPreFilter.Where(a => a.DateCreated >= localDate).ToList();
                    break;
            }

            var completed = orders.Where(c => c.OrderTrackings.Any(d => d.StatusCode.IsComplete))
                .Select(e => new { e.CreatedBy, Pending = 0, Completed = e.TotalFromDb }).ToList()
                .GroupBy(f => f.CreatedBy).Select(g => new { id = g.Key, Pending = 0m, Completed = g.Sum(s => s.Completed) }).ToList();


            var open = orders.Where(c => !c.OrderTrackings.Any(d => d.StatusCode.IsComplete))
                .Select(e => new { e.CreatedBy, Pending = e.TotalFromDb, Completed = 0 }).ToList()
                .GroupBy(f => f.CreatedBy).Select(g => new { id = g.Key, Pending = g.Sum(s => s.Pending), Completed = 0m }).ToList();

            var rtValue = completed.Union(open).GroupBy(a => a.id).Select(b => new RequesterTotals() { FullNameAndId = b.Key.FullNameAndId, Pending = string.Format("{0:C}", b.Sum(s => s.Pending)), Completed = string.Format("{0:C}", b.Sum(s => s.Completed))}).ToList();
            var odd = false;
            foreach (var requesterTotal in rtValue)
            {
                requesterTotal.EvenOdd = odd ? "odd" : "even";
                odd = !odd;
            }

            return rtValue;
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

    public class RequesterTotals
    {
        public string FullNameAndId { get; set; }
        public string Pending { get; set; }
        public string Completed { get; set; }
        public string EvenOdd { get; set; }
    }




  
	
}
