using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MvcContrib;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;
using Microsoft.Practices.ServiceLocation;

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

        public ActionResult ViewHistory()
        {
            var indexService = ServiceLocator.Current.GetInstance<Purchasing.Web.Services.IIndexService>();
            var start = DateTime.Now;
            var myOrderIds =
                _queryRepositoryFactory.AccessRepository.Queryable.Where(
                    x => x.AccessUserId == CurrentUser.Identity.Name).Select(x => x.OrderId).ToArray();
            ViewBag.OrderQuery = (DateTime.Now - start).Milliseconds;

            start = DateTime.Now;
            var orderHistory = indexService.GetOrderHistory(new int[] {1, 2, 3, 4, 5, 6});
            ViewBag.IndexQuery = (DateTime.Now - start).Milliseconds;

            return View(orderHistory);
        }

        [Authorize]
        public ActionResult Landing()
        {
            if (!_userRepository.Queryable.Any(a => a.Id == CurrentUser.Identity.Name && a.IsActive))
            {
                Message = "You are currently not an active user for this program. If you believe this is incorrect contact your departmental administrator to add you.";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
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
    }
}