using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcContrib;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;

        public HomeController(IRepositoryWithTypedId<User, string> userRepository, IQueryRepositoryFactory queryRepositoryFactory)
        {
            _userRepository = userRepository;
            _queryRepositoryFactory = queryRepositoryFactory;
        }

        /// <summary>
        /// Landing Page welcoming Users to the PrePurchasing System
        /// </summary>
        /// <returns></returns>
        [HandleTransactionsManually]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Landing()
        {
            if (!_userRepository.Queryable.Any(a => a.Id == CurrentUser.Identity.Name && a.IsActive))
            {
                Message = "You are currently not an active user for this program. If you believe this is incorrect contact your departmental administrator to add you.";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized()); //TODO: use http unauthorized?
            }

            var viewModel = new LandingViewModel
                                {
                                    PendingOrders = _queryRepositoryFactory.PendingOrderRepository
                                                        .Queryable
                                                        .Where(x=>x.AccessUserId == CurrentUser.Identity.Name)
                                                        .OrderByDescending(x=>x.LastActionDate)
                                                        .Select(x=>(OrderHistoryBase)x).ToFuture(),
                                    YourOpenOrders = _queryRepositoryFactory.OpenOrderByUserRepository
                                                        .Queryable
                                                        .Where(x => x.AccessUserId == CurrentUser.Identity.Name)
                                                        .OrderByDescending(x => x.LastActionDate)
                                                        .Select(x => (OrderHistoryBase)x).ToFuture().ToList()
                                };

            return View(viewModel);
        }

        /// <summary>
        /// Ajax call
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Authorize]
        public JsonNetResult RequestAmountSummaryCall(string filter)
        {
            return new JsonNetResult(GetRequesterTotals(filter));
        }
        
        [HandleTransactionsManually]
        public ActionResult About()
        {
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