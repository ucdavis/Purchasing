using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    [HandleTransactionsManually] //Don't create transactions for home controller methods
    public class HomeController : ApplicationController
    {
        private readonly IOrderAccessService _orderAccessService;

        public HomeController(IOrderAccessService orderAccessService)
        {
            _orderAccessService = orderAccessService;
        }

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
        public JsonNetResult RequestAmountSummaryCall(string filter)
        {
            return new JsonNetResult(GetRequesterTotals(filter));
        }

        /// <summary>
        /// TODO: Remove this page before going to production
        /// </summary>
        /// <returns></returns>
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


        #region Private Methods
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

            var rtValue = completed.Union(open).GroupBy(a => a.id).Select(b => new RequesterTotals() { FullNameAndId = b.Key.FullNameAndId, Pending = string.Format("{0:C}", b.Sum(s => s.Pending)), Completed = string.Format("{0:C}", b.Sum(s => s.Completed)) }).ToList();
            var odd = false;
            foreach(var requesterTotal in rtValue)
            {
                requesterTotal.EvenOdd = odd ? "odd" : "even";
                odd = !odd;
            }

            return rtValue;
        }

        private List<int> GetListOfOrderIds(int id)
        {
            var dt = DateTime.Now.AddDays(-id).Date;

            var orderIds = (Repository.OfType<OrderTracking>().Queryable
                .Where(a => a.Order.CreatedBy.Id == CurrentUser.Identity.Name)
                .GroupBy(s => s.Order.Id).Select(g => new { id = g.Key, LastDateActedOn = g.Max(s => s.DateCreated) }).ToList().
                Where(xy => xy.LastDateActedOn <= dt)).Select(cx => cx.id).ToList();
            return orderIds;
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

        #endregion Private Methods
    }
}