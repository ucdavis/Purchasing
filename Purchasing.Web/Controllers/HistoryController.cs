using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// List of orders
        /// </summary>
        /// <param name="selectedOrderStatus"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="showPending"></param>
        /// <param name="showLast"></param>
        /// <returns></returns>
        public ActionResult Index(string selectedOrderStatus, DateTime? startDate, DateTime? endDate, DateTime? startLastActionDate, DateTime? endLastActionDate, bool showPending = false, bool showCreated = false) //, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideOrdersYouCreated = false)
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


            var orders = _orderService.GetListofOrders(isComplete, showPending, selectedOrderStatus, startDate, endDate, showCreated, startLastActionDate, endLastActionDate);


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

            PopulateModel(orders.OrderByDescending(a=> a.LastActionDate).ToList(), model);

            return View(model);

        }

        /// <summary>
        /// Page to view Administrative Workgroup Orders
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminOrders(string selectedOrderStatus, DateTime? startDate, DateTime? endDate, DateTime? startLastActionDate, DateTime? endLastActionDate, bool showPending = false)
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

            var orders = _orderService.GetAdministrativeListofOrders(isComplete, showPending, selectedOrderStatus, startDate, endDate);

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

            return View(model);
        }


        #region PartialViews

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