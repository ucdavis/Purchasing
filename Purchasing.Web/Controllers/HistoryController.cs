using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Models;
using Purchasing.Web.Services;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Disabling session state #1 because we don't need it, #2 so that we can call partial views and ajax calls in parallel (aka no lock on session)
    /// </summary>
    public class HistoryController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IOrderService _orderService;

        public HistoryController(IRepositoryFactory repositoryFactory, IOrderService orderService)
        {
            _repositoryFactory = repositoryFactory;
            _orderService = orderService;
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
            if (showPending == false && selectedOrderStatus == null && startDate == null && endDate == null && startLastActionDate == null && endLastActionDate == null)
            {
                startLastActionDate = DateTime.Now.AddDays(-90);
            }
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
           // var saveSelectedOrderStatus = selectedOrderStatus;
            if (selectedOrderStatus == "All")
            {
                selectedOrderStatus = null;
            }

            var isComplete = (selectedOrderStatus == OrderStatusCode.Codes.Complete);

            string received = null;
            if (selectedOrderStatus == "Received" || selectedOrderStatus == "UnReceived")
            {
                if (selectedOrderStatus == "Received")
                {
                    received = "yes";
                }
                else if (selectedOrderStatus == "UnReceived")
                {
                    received = "no";
                }
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }

            string paid = null;
            if (selectedOrderStatus == "Paid" || selectedOrderStatus == "UnPaid")
            {
                if (selectedOrderStatus == "Paid")
                {
                    paid = "yes";
                }
                else if (selectedOrderStatus == "UnPaid")
                {
                    paid = "no";
                }
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }

            var ordersIndexed = _orderService.GetIndexedListofOrders(received, paid, isComplete, showPending, selectedOrderStatus, startDate, endDate, showCreated, startLastActionDate, endLastActionDate);
            ViewBag.IndexLastModified = ordersIndexed.LastModified;

            var orders = ordersIndexed.Results.AsQueryable();


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

            PopulateModel(orders.ToList(), model);
            if (model.OrderHistory.Count >= 1000)
            {
                Message = "We are only displaying the 1,000 most recently acted on orders, so there may be older orders which are not included.  Adjust your filters to be more specific or use the “Search Your Orders” feature to find those much older orders if necessary.";
            }

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
            if (showPending == false && selectedOrderStatus == null && startDate == null && endDate == null && startLastActionDate == null && endLastActionDate == null)
            {
                startLastActionDate = DateTime.Now.AddDays(-31);
            }
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            if (selectedOrderStatus == "All")
            {
                selectedOrderStatus = null;
            }
            var isComplete = selectedOrderStatus == OrderStatusCode.Codes.Complete;

            string received = null;
            if (selectedOrderStatus == "Received" || selectedOrderStatus == "UnReceived")
            {
                if (selectedOrderStatus == "Received")
                {
                    received = "yes";
                }
                else if (selectedOrderStatus == "UnReceived")
                {
                    received = "no";
                }
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }

            string paid = null;
            if (selectedOrderStatus == "Paid" || selectedOrderStatus == "UnPaid")
            {
                if (selectedOrderStatus == "Paid")
                {
                    paid = "yes";
                }
                else if (selectedOrderStatus == "UnPaid")
                {
                    paid = "no";
                }
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }

            var ordersIndexed = _orderService.GetAdministrativeIndexedListofOrders(received, paid, isComplete, showPending, selectedOrderStatus, startDate, endDate, startLastActionDate, endLastActionDate);
            ViewBag.IndexLastModified = ordersIndexed.LastModified;
            
            var orders = ordersIndexed.Results.AsQueryable();



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
            PopulateModel(orders.ToList(), model);

            if (model.OrderHistory.Count >= 1000)
            {
                Message = "We are only displaying the 1,000 most recently acted on orders, so there may be older orders which are not included.  Adjust your filters to be more specific or use the “Search Your Orders” feature to find those much older orders if necessary.";
            }

            return View("AdminOrders", model);
        }

        #region Private Methods

        private void PopulateModel(List<OrderHistory> orders, FilteredOrderListModelDto model)
        {
            model.OrderHistory = orders;

            model.PopulateStatusCodes(_repositoryFactory.OrderStatusCodeRepository);
        } 

        #endregion Private Methods
    }
}