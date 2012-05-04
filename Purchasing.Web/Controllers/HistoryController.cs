using System;
using System.Linq;
using Purchasing.Core;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Services;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    [Authorize]
    public class HistoryController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IOrderService _orderService;

        public HistoryController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IOrderService OrderService)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _orderService = OrderService;
        }
        #region partialViews

        public ActionResult RecentActivity()
        {
            var lastOrderEvent = _queryRepositoryFactory.OrderTrackingHistoryRepository.Queryable.Where(
                d => d.AccessUserId == CurrentUser.Identity.Name).OrderByDescending(e => e.DateCreated).FirstOrDefault();

            return PartialView(lastOrderEvent);
        }

        public ActionResult RecentComments()
        {
            var recentComments = Repository.OfType<CommentHistory>()
                .Queryable.Where(a => a.AccessUserId == CurrentUser.Identity.Name)
                .OrderByDescending(o => o.DateCreated)
                .Take(5).ToList();

            return PartialView(recentComments);
        }


        #endregion PartialViews

        #region AJAX Calls
        public JsonNetResult RecentlyCompleted()
        {

            var deniedThisMonth =
                _orderService.GetListofOrders(false, false, OrderStatusCode.Codes.Denied, null, null, true,
                                              new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), null).Count();
            var completedThisMonth =
                _orderService.GetListofOrders(true, false, OrderStatusCode.Codes.Complete, null, null, true,
                                  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), null).Count();

            return new JsonNetResult(new {deniedThisMonth, completedThisMonth});
        }
         

        #endregion AJAX Calls

        //public ActionResult Orders(string selectedOrderStatus, DateTime? startCreateDate, DateTime? endCreateDate, DateTime? startLastActionDate, DateTime? endLastActionDate, bool showPending = false, bool showCreatedByUser = false)
        //{
        //    //var orders = _queryRepositoryFactory.OrderHistoryRepository.Queryable.Where(a=> a.AccessUserId==CurrentUser.Identity.Name && !a.IsAdmin).Distinct();
        //    //if (selectedOrderStatus=="Received")
        //    //{
        //    //    orders = orders.Where(a => a.Received == "Yes");
        //    //} else if (selectedOrderStatus=="UnReceived")
        //    //{
        //    //    orders = orders.Where(a => a.Received == "No" && a.IsComplete);
        //    //} else if (selectedOrderStatus != "All")
        //    //{
        //    //    orders = orders.Where(a => a.Status == selectedOrderStatus);
        //    //}
            
        //    //if (showPending)
        //    //{
        //    //    orders
        //    //}

        //}
    }
}