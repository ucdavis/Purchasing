using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;
using IronRuby.Builtins;
using Purchasing.Core;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Disabling session state #1 because we don't need it, #2 so that we can call partial views and ajax calls in parallel (aka no lock on session)
    /// </summary>
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

        /// <summary>
        /// List of Orders
        /// </summary>
        /// <param name="selectedOrderStatus">Includes 3 that are not in our database</param>
        /// <param name="startDate">For when Order was created</param>
        /// <param name="endDate">For when Order was created</param>
        /// <param name="startLastActionDate">For when Order was last acted on</param>
        /// <param name="endLastActionDate">For when Order was last acted on</param>
        /// <param name="showPending">Show orders pending your action</param>
        /// <param name="showCreated">Only show orders you created</param>
        /// <returns></returns>
        public ActionResult Index(string selectedOrderStatus,
            DateTime? startDate,
            DateTime? endDate,
            DateTime? startLastActionDate,
            DateTime? endLastActionDate,
            bool showPending = false,
            bool showCreated = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            var saveSelectedOrderStatus = selectedOrderStatus;
            if (selectedOrderStatus == "All")
            {
                selectedOrderStatus = null;
            }

            var isComplete = (selectedOrderStatus == OrderStatusCode.Codes.Complete);

            if (selectedOrderStatus == "Received" || selectedOrderStatus == "UnReceived")
            {
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }

            var ordersIndexed = _orderService.GetIndexedListofOrders(isComplete, showPending, selectedOrderStatus, startDate, endDate, showCreated, startLastActionDate, endLastActionDate);
            ViewBag.IndexLastModified = ordersIndexed.LastModified;

            var orders = ordersIndexed.Results.AsQueryable();

            if (saveSelectedOrderStatus == "Received")
            {
                orders = orders.Where(a => a.Received == "Yes");
            }
            else if (saveSelectedOrderStatus == "UnReceived")
            {
                orders = orders.Where(a => a.Received == "No");
            }

            var model = new FilteredOrderListModelDto
            {
                SelectedOrderStatus = selectedOrderStatus,
                StartDate = startDate,
                EndDate = endDate,
                StartLastActionDate = startLastActionDate,
                EndLastActionDate = endLastActionDate,
                ShowPending = showPending,
                ShowCreated = showCreated,
                ColumnPreferences =
                    _repositoryFactory.ColumnPreferencesRepository.GetNullableById(
                        CurrentUser.Identity.Name) ??
                    new ColumnPreferences(CurrentUser.Identity.Name)
            };
            ViewBag.DataTablesPageSize = model.ColumnPreferences.DisplayRows;

            PopulateModel(orders.OrderByDescending(a => a.LastActionDate).ToList(), model);

            return View("Index", model);
        }

        /// <summary>
        /// Page to view Administrative Workgroup Orders
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Page to view Administrative Workgroup Orders
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminOrders(string selectedOrderStatus,
            DateTime? startDate,
            DateTime? endDate,
            DateTime? startLastActionDate,
            DateTime? endLastActionDate,
            bool showPending = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            var saveSelectedOrderStatus = selectedOrderStatus;
            if (selectedOrderStatus == "All")
            {
                selectedOrderStatus = null;
            }
            var isComplete = selectedOrderStatus == OrderStatusCode.Codes.Complete;

            if (selectedOrderStatus == "Received" || selectedOrderStatus == "UnReceived")
            {
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }

            var ordersIndexed = _orderService.GetAdministrativeIndexedListofOrders(isComplete, showPending, selectedOrderStatus, startDate, endDate, startLastActionDate, endLastActionDate);
            ViewBag.IndexLastModified = ordersIndexed.LastModified;
            
            var orders = ordersIndexed.Results.AsQueryable();

            if (saveSelectedOrderStatus == "Received")
            {
                orders = orders.Where(a => a.Received == "Yes");
            }
            else if (saveSelectedOrderStatus == "UnReceived")
            {
                orders = orders.Where(a => a.Received == "No");
            }

            var model = new FilteredOrderListModelDto
            {
                SelectedOrderStatus = selectedOrderStatus,
                StartDate = startDate,
                EndDate = endDate,
                StartLastActionDate = startLastActionDate,
                EndLastActionDate = endLastActionDate,
                ShowPending = showPending,
                ColumnPreferences =
                    _repositoryFactory.ColumnPreferencesRepository.GetNullableById(
                        CurrentUser.Identity.Name) ??
                    new ColumnPreferences(CurrentUser.Identity.Name)
            };
            ViewBag.DataTablesPageSize = model.ColumnPreferences.DisplayRows;
            PopulateModel(orders.OrderByDescending(a => a.LastActionDate).ToList(), model);

            return View("AdminOrders", model);
        }

        #region PartialViews

        public ActionResult RecentActivity()
        {
            throw new NotImplementedException("Implemented in the historyajax controller");
        }

        public ActionResult RecentComments()
        {
            throw new NotImplementedException("Implemented in the historyajax controller");
        }


        #endregion PartialViews

        #region AJAX Calls
        public JsonNetResult RecentlyCompleted()
        {
            throw new NotImplementedException("Implemented in the historyajax controller");
        }
         

        #endregion AJAX Calls

        #region Private Methods

        private void PopulateModel(List<OrderHistory> orders, FilteredOrderListModelDto model)
        {
            model.OrderHistory = orders;

            if (model.RequresOrderTracking() || model.RequiresApprovals())
            {
                var orderIds = model.OrderHistory.Select(a => a.OrderId).ToList();
                if (model.RequresOrderTracking())
                {
                    model.OrderTracking =
                        (from o in
                             _repositoryFactory.OrderTrackingRepository.Queryable.Fetch(x => x.User).Fetch(
                                 x => x.StatusCode)
                         where orderIds.Contains(o.Order.Id)
                         select o).ToList();
                }

                if (model.RequiresApprovals())
                {
                    model.Approvals =
                        (from a in
                             _repositoryFactory.ApprovalRepository.Queryable.Fetch(x => x.User).Fetch(
                                 x => x.SecondaryUser)
                         where orderIds.Contains(a.Order.Id)
                         select a).ToList();
                }
            }

            model.PopulateStatusCodes(_repositoryFactory.OrderStatusCodeRepository);
        } 

        #endregion Private Methods
    }
}