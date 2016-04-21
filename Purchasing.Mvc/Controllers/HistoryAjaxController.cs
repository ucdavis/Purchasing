using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Dapper;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Controllers;
using UCDArch.Web.ActionResults;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for historical order ajax methods that don't require session state.  this way they can run in parallel.
    /// </summary>
    [SessionState(SessionStateBehavior.Disabled)]
    public class HistoryAjaxController : ApplicationController
    {
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IOrderService _orderService;
        private readonly IDbService _dbService;

        public HistoryAjaxController(IQueryRepositoryFactory queryRepositoryFactory, IOrderService orderService, IDbService dbService)
        {
            _queryRepositoryFactory = queryRepositoryFactory;
            _orderService = orderService;
            _dbService = dbService;
        }

        public PartialViewResult RecentActivity()
        {
            using (var conn = _dbService.GetConnection())
            {
                var lastOrderEvent = conn.Query<OrderTrackingHistory>(
                    "SELECT * FROM [dbo].[udf_GetPendingOrdersForLogin] (@login) ORDER BY lastactiondate DESC",
                    new {login = CurrentUser.Identity.Name}).FirstOrDefault();

                return PartialView(lastOrderEvent);
            }
        }

        public PartialViewResult RecentComments()
        {
            var recentComments = _queryRepositoryFactory.CommentHistoryRepository
                .Queryable.Where(a => a.AccessUserId == CurrentUser.Identity.Name)
                .OrderByDescending(o => o.DateCreated)
                .Take(5).ToList();

            return PartialView(recentComments);
        }

        public JsonNetResult RecentlyCompleted()
        {
            var deniedThisMonth =
                _orderService.GetIndexedListofOrders(null, null,false, false, OrderStatusCode.Codes.Denied, null, null, true,
                                              new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null).Results.Count();
            var completedThisMonth =
                _orderService.GetIndexedListofOrders(null, null,true, false, OrderStatusCode.Codes.Complete, null, null, true,
                                  new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null).Results.Count();

            return new JsonNetResult(new { deniedThisMonth, completedThisMonth });
        }
	}
}
