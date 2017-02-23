﻿using System;
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
        private readonly ISearchService _searchService;
        private readonly IAccessQueryService _accessQueryService;
        private readonly IOrderService _orderService;
        private readonly IDbService _dbService;

        public HistoryAjaxController(ISearchService searchService, IAccessQueryService accessQueryService, IOrderService orderService, IDbService dbService)
        {
            _searchService = searchService;
            _accessQueryService = accessQueryService;
            _orderService = orderService;
            _dbService = dbService;
        }

        public PartialViewResult RecentActivity()
        {
            using (var conn = _dbService.GetConnection())
            {
                var lastOrderEvent = conn.Query<OrderTrackingHistory>(
                    "SELECT * FROM [dbo].[udf_GetRecentActivityForLogin] (@login)",
                    new {login = CurrentUser.Identity.Name}).FirstOrDefault();

                return PartialView(lastOrderEvent);
            }
        }

        public PartialViewResult RecentComments()
        {
            var orderIds = _accessQueryService.GetOrderAccessByAdminStatus(CurrentUser.Identity.Name, isAdmin: false).Select(x => x.OrderId).ToArray();

            var comments = _searchService.GetLatestComments(5, orderIds);

            return PartialView(comments);
        }

        public JsonNetResult RecentlyCompleted()
        {
            var accessibleOrders = _accessQueryService.GetOrderAccessByAdminStatus(CurrentUser.Identity.Name, isAdmin: false).ToArray();
            
            var deniedThisMonth =
                _orderService.GetIndexedListofOrders(accessibleOrders, null, null,false, false, OrderStatusCode.Codes.Denied, null, null, true,
                                              new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null).Results.Count();
            var completedThisMonth =
                _orderService.GetIndexedListofOrders(accessibleOrders, null, null,true, false, OrderStatusCode.Codes.Complete, null, null, true,
                                  new DateTime(DateTime.UtcNow.ToPacificTime().Year, DateTime.UtcNow.ToPacificTime().Month, 1), null).Results.Count();

            return new JsonNetResult(new { deniedThisMonth, completedThisMonth });
        }
	}
}
