﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using UCDArch.Web.ActionResults;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Report class
    /// </summary>
    [Authorize]
    public class ReportController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IReportRepositoryFactory _reportRepositoryFactory;
        private readonly IReportService _reportService;
        private readonly IWorkgroupService _workgroupService;
        private readonly ISearchService _searchService;


        public ReportController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory,
            IReportRepositoryFactory reportRepositoryFactory, IReportService reportService,
            IWorkgroupService workgroupService, ISearchService searchService)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _reportRepositoryFactory = reportRepositoryFactory;
            _reportService = reportService;
            _workgroupService = workgroupService;
            _searchService = searchService;
        }

        [AuthorizeReadOrEditOrder]
        public Microsoft.AspNetCore.Mvc.FileResult Invoice(int id, bool showOrderHistory = false, bool forVendor = false)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            var invoice = _reportService.GetInvoice(order, showOrderHistory, forVendor);

            return File(invoice, "application/pdf");
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult PurchaserWorkLoad(DateTime? reportDate)
        {
            reportDate = reportDate ?? DateTime.UtcNow.ToPacificTime();

            var viewModel = new ReportPurchaserWorkLoadViewModel();
            viewModel.Items = new List<ReportPurchaserWorkLoadItem>();
            viewModel.ReportDate = reportDate;

            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true).ToList();
            var workgroupIds = allWorkgroups.Select(x => x.Id).ToArray();

            var users =
                _repositoryFactory.OrderTrackingRepository
                    .Queryable.Where(
                        a =>
                            a.DateCreated >= reportDate.Value.Date && a.DateCreated <= reportDate.Value.Date.AddDays(1) &&
                            workgroupIds.Contains(a.Order.Workgroup.Id) && a.Description.Contains("completed"))
                    .Select(a => a.User).ToList();
            var distinctUsers = users.Distinct().ToList();
            foreach (var user in distinctUsers)
            {
                var item = new ReportPurchaserWorkLoadItem();
                item.userId = user.Id;
                item.UserName = user.FullName;
                item.CompletedCount = users.Count(a => a.Id == user.Id);
                    //This will tell me how many orders have been completed by each purchaser.
                viewModel.Items.Add(item);
            }


            var userNamesFromOrderHistory =
                _queryRepositoryFactory.OrderHistoryRepository.Queryable.Where(
                    a => a.StatusId == "PR" && workgroupIds.Contains(a.WorkgroupId)).Select(b => b.Purchaser).ToList();
            var names = userNamesFromOrderHistory.Distinct().ToList();
            foreach (var userName in names)
            {
                var userName1 = userName.Trim();
                var item = viewModel.Items.SingleOrDefault(a => a.UserName == userName1);
                if (item == null)
                {
                    item = new ReportPurchaserWorkLoadItem();
                    item.UserName = userName1;
                    item.PendingCount = userNamesFromOrderHistory.FindAll(a => a.Contains(userName1)).Count;
                        //Find all pending orders in Purchaser state
                    viewModel.Items.Add(item);
                }
                else
                {
                    item.PendingCount = userNamesFromOrderHistory.FindAll(a => a.Contains(userName)).Count;
                        //Find all pending orders in Purchaser state
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult Workload(int? workgroupId = null)
        {
            Workgroup workgroup = null;

            if (workgroupId.HasValue)
            {
                workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId.Value);
            }

            var viewModel = ReportWorkloadViewModel.Create(_repositoryFactory, _queryRepositoryFactory,
                _workgroupService, CurrentUser.Identity.Name, workgroup);

            if (workgroupId.HasValue)
            {
                viewModel.GenerateDisplayTable(_reportRepositoryFactory, workgroupId.Value);
            }

            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        public ActionResult Permissions(bool hideInherited = false)
        {
            var viewModel = ReportPermissionsViewModel.Create(_repositoryFactory, _workgroupService, hideInherited);

            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult TotalByWorkgroup(DateTime? startDate, DateTime? endDate, bool showAdmin)
        {
            const int defaultResultSize = 1000;
            var viewModel = TotalByWorkgroupViewModel.Create(startDate, endDate, showAdmin);

            if (startDate == null || endDate == null)
            {
                Message = "Select a start date and an end date then click apply to view report.";


                return View(viewModel);
            }

            //Grab all workgroups for this user as well as related primary orgs
            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true).ToList();
            var workgroupIds = allWorkgroups.Select(x => x.Id).ToArray();
            var workgroupsWithPrimaryOrgs =
                _repositoryFactory.WorkgroupRepository.Queryable.Where(x => workgroupIds.Contains(x.Id))
                    .Select(x => new {Workgroup = x, Primary = x.PrimaryOrganization}).ToList();

            //Get every order that matches these workgroups
            var matchingOrders = _searchService.GetOrdersByWorkgroups(allWorkgroups, startDate.Value, endDate.Value, defaultResultSize);
            if (matchingOrders.Count == defaultResultSize)
            {
                Message = "Max size of 1000 reached. Please adjust filters.";
            }
            var workgroupCounts = new List<OrderTotals>();
            foreach (var workgroup in allWorkgroups)
            {
                if (workgroup.IsActive && !workgroup.Administrative)
                {
                    var orderTotal = new OrderTotals();
                    orderTotal.WorkgroupName = workgroup.Name;
                    orderTotal.WorkgroupId = workgroup.Id;
                    orderTotal.Administrative = false;
                    orderTotal.PrimaryOrg =
                        workgroupsWithPrimaryOrgs.Single(x => x.Workgroup.Id == workgroup.Id).Primary.Name;
                    orderTotal.InitiatedOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id);
                    orderTotal.DeniedOrders =
                        matchingOrders.AsQueryable()
                            .Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Denied);
                    orderTotal.CanceledOrders =
                        matchingOrders.AsQueryable()
                            .Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Cancelled);
                    orderTotal.CompletedOrders =
                        matchingOrders.AsQueryable()
                            .Count(
                                a =>
                                    a.WorkgroupId == workgroup.Id &&
                                    (a.StatusId == OrderStatusCode.Codes.Complete ||
                                     a.StatusId == OrderStatusCode.Codes.CompleteNotUploadedKfs));
                    orderTotal.PendingOrders =
                        matchingOrders.AsQueryable()
                            .Count(
                                a =>
                                    a.WorkgroupId == workgroup.Id &&
                                    (a.StatusId == OrderStatusCode.Codes.Approver ||
                                     a.StatusId == OrderStatusCode.Codes.AccountManager ||
                                     a.StatusId == OrderStatusCode.Codes.Purchaser ||
                                     a.StatusId == OrderStatusCode.Codes.Requester ||
                                     a.StatusId == OrderStatusCode.Codes.ConditionalApprover));

                    workgroupCounts.Add(orderTotal);
                }
            }
            if (showAdmin)
            {
                foreach (var workgroup in allWorkgroups)
                {
                    if (workgroup.IsActive && workgroup.Administrative)
                    {
                        var orderTotal = new OrderTotals();
                        orderTotal.WorkgroupName = workgroup.Name;
                        orderTotal.WorkgroupId = workgroup.Id;
                        orderTotal.Administrative = true;
                        orderTotal.PrimaryOrg = workgroup.PrimaryOrganization.Name;

                        orderTotal.InitiatedOrders = 0;
                        orderTotal.DeniedOrders = 0;
                        orderTotal.CanceledOrders = 0;
                        orderTotal.CompletedOrders = 0;
                        orderTotal.PendingOrders = 0;
                        var childWorkgroupIds = _workgroupService.GetChildWorkgroups(workgroup.Id);

                        foreach (var childWorkgroupId in childWorkgroupIds)
                            //NOTE these child workgroup Id's don't include ones that "do not inherit permissions"
                        {
                            int id = childWorkgroupId;
                            var childOrderTotal = workgroupCounts.FirstOrDefault(a => a.WorkgroupId == id);
                            if (childOrderTotal != null)
                            {
                                orderTotal.InitiatedOrders += childOrderTotal.InitiatedOrders;
                                orderTotal.DeniedOrders += childOrderTotal.DeniedOrders;
                                orderTotal.CanceledOrders += childOrderTotal.CanceledOrders;
                                orderTotal.CompletedOrders += childOrderTotal.CompletedOrders;
                                orderTotal.PendingOrders += childOrderTotal.PendingOrders;
                            }
                        }
                        workgroupCounts.Add(orderTotal);
                    }
                }
            }
            viewModel.WorkgroupCounts = workgroupCounts.OrderBy(a => a.WorkgroupName);


            var columnPreferences =
                _repositoryFactory.ColumnPreferencesRepository.GetNullableById(CurrentUser.Identity.Name) ??
                new ColumnPreferences(CurrentUser.Identity.Name);

            ViewBag.DataTablesPageSize = columnPreferences.DisplayRows;

            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult TotalByPrimaryOrg(DateTime? startDate, DateTime? endDate)
        {
            const int defaultResultSize = 1000;
            var viewModel = TotalByWorkgroupViewModel.Create(startDate, endDate, false);

            if (startDate == null || endDate == null)
            {
                Message = "Select a start date and an end date then click apply to view report.";

                return View(viewModel);
            }

            //Grab all workgroups for this user as well as related primary orgs
            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true).ToList();
            var workgroupIds = allWorkgroups.Select(x => x.Id).ToArray();
            var workgroupsWithPrimaryOrgs =
                _repositoryFactory.WorkgroupRepository.Queryable.Where(x => workgroupIds.Contains(x.Id))
                    .Select(x => new {Workgroup = x, Primary = x.PrimaryOrganization}).ToList();

            //Get every order that matches these workgroups
            var matchingOrders = _searchService.GetOrdersByWorkgroups(allWorkgroups, startDate.Value, endDate.Value, defaultResultSize);
            if (matchingOrders.Count == defaultResultSize)
            {
                Message = "Max size of 1000 reached. Please adjust filters.";
            }
            var workgroupCounts = new List<OrderTotals>();

            foreach (var workgroup in allWorkgroups)
            {
                if (workgroup.IsActive && !workgroup.Administrative)
                {
                    var orderTotal =
                        workgroupCounts.FirstOrDefault(a => a.PrimaryOrg == workgroup.PrimaryOrganization.Name);
                    if (orderTotal == null)
                    {
                        orderTotal = new OrderTotals();
                        orderTotal.PrimaryOrg =
                            workgroupsWithPrimaryOrgs.Single(x => x.Workgroup.Id == workgroup.Id).Primary.Name;
                        orderTotal.InitiatedOrders = 0;
                        orderTotal.DeniedOrders = 0;
                        orderTotal.CanceledOrders = 0;
                        orderTotal.CompletedOrders = 0;
                        orderTotal.PendingOrders = 0;

                        workgroupCounts.Add(orderTotal);
                    }

                    orderTotal.InitiatedOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id);
                    orderTotal.DeniedOrders +=
                        matchingOrders.AsQueryable()
                            .Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Denied);
                    orderTotal.CanceledOrders +=
                        matchingOrders.AsQueryable()
                            .Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Cancelled);
                    orderTotal.CompletedOrders +=
                        matchingOrders.AsQueryable()
                            .Count(
                                a =>
                                    a.WorkgroupId == workgroup.Id &&
                                    (a.StatusId == OrderStatusCode.Codes.Complete ||
                                     a.StatusId == OrderStatusCode.Codes.CompleteNotUploadedKfs));
                    orderTotal.PendingOrders +=
                        matchingOrders.AsQueryable()
                            .Count(
                                a =>
                                    a.WorkgroupId == workgroup.Id &&
                                    (a.StatusId == OrderStatusCode.Codes.Approver ||
                                     a.StatusId == OrderStatusCode.Codes.AccountManager ||
                                     a.StatusId == OrderStatusCode.Codes.Purchaser ||
                                     a.StatusId == OrderStatusCode.Codes.Requester ||
                                     a.StatusId == OrderStatusCode.Codes.ConditionalApprover));

                }
            }

            viewModel.WorkgroupCounts = workgroupCounts.OrderBy(a => a.PrimaryOrg);

            var columnPreferences =
                _repositoryFactory.ColumnPreferencesRepository.GetNullableById(CurrentUser.Identity.Name) ??
                new ColumnPreferences(CurrentUser.Identity.Name);
            ViewBag.DataTablesPageSize = columnPreferences.DisplayRows;

            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult TotalByVendor(DateTime? startDate, DateTime? endDate)
        {
            const int defaultResultSize = 1000;
            var viewModel = TotalByWorkgroupViewModel.Create(startDate, endDate, false);

            if (startDate == null || endDate == null)
            {
                Message = "Select a start date and an end date then click apply to view report.";

                return View(viewModel);
            }

            //Grab all workgroups for this user
            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true).ToList();

            //Get every order that matches these workgroups
            var matchingOrders = _searchService.GetOrdersByWorkgroups(allWorkgroups, startDate.Value, endDate.Value, defaultResultSize);
            if (matchingOrders.Count == defaultResultSize)
            {
                Message = "Max size of 1000 reached. Please adjust filters.";
            }
            var workgroupCounts = new List<OrderTotals>();

            foreach (var workgroup in allWorkgroups)
            {
                if (workgroup.IsActive && !workgroup.Administrative)
                {
                    var orders = matchingOrders.AsQueryable().Where(a => a.WorkgroupId == workgroup.Id);

                    foreach (var order in orders)
                    {
                        var orderTotal = workgroupCounts.FirstOrDefault(a => a.Vendor == order.Vendor);

                        if (orderTotal == null)
                        {
                            orderTotal = new OrderTotals();
                            orderTotal.Vendor = order.Vendor;
                            orderTotal.InitiatedOrders = 0;
                            orderTotal.DeniedOrders = 0;
                            orderTotal.CanceledOrders = 0;
                            orderTotal.CompletedOrders = 0;
                            orderTotal.PendingOrders = 0;

                            workgroupCounts.Add(orderTotal);
                        }
                        orderTotal.InitiatedOrders++;
                        switch (order.StatusId)
                        {
                            case OrderStatusCode.Codes.Denied:
                                orderTotal.DeniedOrders++;
                                break;
                            case OrderStatusCode.Codes.Cancelled:
                                orderTotal.CanceledOrders++;
                                break;
                            case OrderStatusCode.Codes.Complete:
                            case OrderStatusCode.Codes.CompleteNotUploadedKfs:
                                orderTotal.CompletedOrders++;
                                break;
                            default:
                                orderTotal.PendingOrders++;
                                break;
                        }
                    }
                }
            }

            viewModel.WorkgroupCounts = workgroupCounts.OrderBy(a => a.Vendor);

            var columnPreferences =
                _repositoryFactory.ColumnPreferencesRepository.GetNullableById(CurrentUser.Identity.Name) ??
                new ColumnPreferences(CurrentUser.Identity.Name);
            ViewBag.DataTablesPageSize = columnPreferences.DisplayRows;

            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult ProcessingTime(int? workgroupId = null, DateTime? month = null,
            bool? onlyShowReRouted = null)
        {
            const int defaultResultSize = 1000;
            Workgroup workgroup = null;

            if (workgroupId.HasValue)
            {
                workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId.Value);
            }
            if (onlyShowReRouted == null)
            {
                onlyShowReRouted = true;
            }
            var viewModel = ReportProcessingTimeViewModel.Create(_workgroupService, workgroup, onlyShowReRouted.Value);

            if (workgroupId.HasValue && month.HasValue)
            {
                viewModel.GenerateDisplayTable(_searchService, _repositoryFactory, _workgroupService, workgroupId.Value,
                    month.Value);
            }
            if (viewModel.SearchResultsCount == defaultResultSize)
            {
                Message = "Max size of 1000 reached. Please adjust filters.";
            }
            

            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult ProcessingTimeSummary(int? workgroupId = null, DateTime? startDate = null,
            DateTime? endDate = null)
        {
            const int defaultResultSize = 1000;
            Workgroup workgroup = null;

            if (workgroupId.HasValue)
            {
                workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId.Value);
            }
           
            if (startDate == null)
            {
                startDate = DateTime.Now.Date.AddDays(-30);
            }
            if (endDate == null)
            {
                endDate = DateTime.Now.Date;
            }
            var workgroups = _workgroupService.LoadAdminWorkgroups().ToList();

            var viewModel = new ReportProcessingTimeSummaryViewModel()
            {
                Workgroups = workgroups,
                Workgroup = workgroup,
                StartDate = startDate,
                EndDate = endDate,
                Columns =
                    _searchService.GetOrderTrackingEntities(
                        workgroup == null ? workgroups.ToArray() : new[] {workgroup}, startDate.Value, endDate.Value, defaultResultSize),
            };

            if (viewModel.Columns.OrderTrackingEntities.Count == defaultResultSize)
            {
                ErrorMessage = string.Format("Max result size of {0} has been reached. Please refine your filters to show complete results. Averages are accurate for the entire set, but not all rows are displayed.", defaultResultSize);
            }
            
            viewModel.JsonData = GetTimeReportData(viewModel.Columns);
            return View(viewModel);
        }

        [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult ProcessingTimeByRole(int? workgroupId = null, DateTime? startDate = null,
            DateTime? endDate = null, string role = "purchaser")
        {
            const int defaultResultSize = 1000;
            Workgroup workgroup = null;

            if (workgroupId.HasValue)
            {
                workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId.Value);
            }

            if (startDate == null)
            {
                startDate = DateTime.Now.AddDays(-30);
            }
            if (endDate == null)
            {
                endDate = DateTime.Now;
            }
            var workgroups = _workgroupService.LoadAdminWorkgroups().ToList();

            var viewModel = new ReportProcessingTimeByRoleViewModel()
            {
                Workgroups = workgroups,
                Workgroup = workgroup,
                Columns =
                    _searchService.GetOrderTrackingEntitiesByRole(
                        workgroup == null ? workgroups.ToArray() : new[] { workgroup }, startDate.Value, endDate.Value, role, defaultResultSize),
                StartDate = startDate,
                EndDate = endDate,
                Role = role
            };

            if (viewModel.Columns.OrderTrackingEntities.Count == defaultResultSize)
            {
                ErrorMessage = string.Format("Max result size of {0} has been reached. Please refine your filters to show complete results belowS. Graph is accurate regardless of hitting this limit.", defaultResultSize);
            }

            //viewModel.JsonData = GetTimeReportData(viewModel.Columns);
            return View(viewModel);
        }

        public dynamic GetTimeReportData(OrderTrackingAggregation data)
        {
            var roles = new string[]
            {
                "approver",
                "account manager",
                "purchaser"
            };

            var approvers = data.OrderTrackingEntities.Where(x=> !string.IsNullOrWhiteSpace(x.ApproverId) && x.MinutesToApprove!=null).Select(x=> new {personId = x.ApproverId, personName= x.ApproverName, minutes =  x.MinutesToApprove, orderId =  x.OrderId });
            var accountManagers = data.OrderTrackingEntities.Where(x => !string.IsNullOrWhiteSpace(x.AccountManagerId) && x.MinutesToAccountManagerComplete!=null).Select(x => new { personId = x.AccountManagerId, personName = x.AccountManagerName, minutes = x.MinutesToAccountManagerComplete, orderId = x.OrderId });
            var purchasers = data.OrderTrackingEntities.Where(x => !string.IsNullOrWhiteSpace(x.PurchaserId) && x.MinutesToPurchaserComplete!=null).Select(x => new { personId = x.PurchaserId, personName = x.PurchaserName, minutes = x.MinutesToPurchaserComplete, orderId = x.OrderId });

            var temp = new {approver = approvers, accountManager = accountManagers, purchaser = purchasers};

            return temp;

        }
    }

    public class ReportProcessingTimeSummaryViewModel
    {
        public IEnumerable<Workgroup> Workgroups;
        public Workgroup Workgroup { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OrderTrackingAggregation Columns { get; set; }
        public bool? OnlyShowCompleted { get; set; }
        public dynamic JsonData { get; set; }
    }

    public class ReportProcessingTimeByRoleViewModel
    {
        public IEnumerable<Workgroup> Workgroups;
        public Workgroup Workgroup { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Role { get; set; }
        public OrderTrackingAggregationByRole Columns { get; set; }
    }

  
}
