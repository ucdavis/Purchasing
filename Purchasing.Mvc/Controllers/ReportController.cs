using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;

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


        public ReportController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IReportRepositoryFactory reportRepositoryFactory, IReportService reportService, IWorkgroupService workgroupService,  ISearchService searchService)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _reportRepositoryFactory = reportRepositoryFactory;
            _reportService = reportService;
            _workgroupService = workgroupService;
            _searchService = searchService;
        }

        [AuthorizeReadOrEditOrder]
        public FileResult Invoice(int id, bool showOrderHistory = false, bool forVendor = false)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            var invoice = _reportService.GetInvoice(order, showOrderHistory, forVendor);

            return File(invoice, "application/pdf");
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
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
                          .Queryable.Where(a => a.DateCreated >= reportDate.Value.Date && a.DateCreated <= reportDate.Value.Date.AddDays(1) && workgroupIds.Contains(a.Order.Workgroup.Id) && a.Description.Contains("completed"))
                          .Select(a => a.User).ToList();
            var distinctUsers = users.Distinct().ToList();
            foreach (var user in distinctUsers)
            {
                var item = new ReportPurchaserWorkLoadItem();
                item.userId = user.Id;
                item.UserName = user.FullName;
                item.CompletedCount = users.Count(a => a.Id == user.Id); //This will tell me how many orders have been completed by each purchaser.
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
                    item.PendingCount = userNamesFromOrderHistory.FindAll(a => a.Contains(userName1)).Count; //Find all pending orders in Purchaser state
                    viewModel.Items.Add(item);
                }
                else
                {
                    item.PendingCount = userNamesFromOrderHistory.FindAll(a => a.Contains(userName)).Count; //Find all pending orders in Purchaser state
                }
            }

            return View(viewModel);
        }

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult Workload(int? workgroupId = null)
        {
            Workgroup workgroup = null;

            if (workgroupId.HasValue)
            {
                workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId.Value);
            }

            var viewModel = ReportWorkloadViewModel.Create(_repositoryFactory, _queryRepositoryFactory, _workgroupService, CurrentUser.Identity.Name, workgroup);

            if (workgroupId.HasValue)
            {
                viewModel.GenerateDisplayTable(_reportRepositoryFactory, workgroupId.Value);
            }
            
            return View(viewModel);
        }

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
        public ActionResult Permissions()
        {
            var viewModel = ReportPermissionsViewModel.Create(_repositoryFactory, _workgroupService);

            return View(viewModel);
        }

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult TotalByWorkgroup(DateTime? startDate, DateTime? endDate, bool showAdmin)
        {
            var viewModel = TotalByWorkgroupViewModel.Create(startDate, endDate, showAdmin);

            if (startDate == null || endDate == null)
            {
                Message = "Select a start date and an end date then click apply to view report.";
                

                return View(viewModel);
            }
            
            //Grab all workgroups for this user as well as related primary orgs
            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true).ToList();
            var workgroupIds = allWorkgroups.Select(x => x.Id).ToArray();
            var workgroupsWithPrimaryOrgs = _repositoryFactory.WorkgroupRepository.Queryable.Where(x => workgroupIds.Contains(x.Id))
                              .Select(x => new { Workgroup = x, Primary = x.PrimaryOrganization }).ToList();
            
            //Get every order that matches these workgroups
            var matchingOrders = _searchService.GetOrdersByWorkgroups(allWorkgroups, startDate.Value, endDate.Value);
            var workgroupCounts = new List<OrderTotals>();
            foreach (var workgroup in allWorkgroups)
            {
                if (workgroup.IsActive && !workgroup.Administrative)
                {
                    var orderTotal = new OrderTotals();
                    orderTotal.WorkgroupName = workgroup.Name;
                    orderTotal.WorkgroupId = workgroup.Id;
                    orderTotal.Administrative = false;
                    orderTotal.PrimaryOrg = workgroupsWithPrimaryOrgs.Single(x => x.Workgroup.Id == workgroup.Id).Primary.Name;
                    orderTotal.InitiatedOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id);
                    orderTotal.DeniedOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Denied);
                    orderTotal.CanceledOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Cancelled);
                    orderTotal.CompletedOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && (a.StatusId == OrderStatusCode.Codes.Complete || a.StatusId == OrderStatusCode.Codes.CompleteNotUploadedKfs));
                    orderTotal.PendingOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && (a.StatusId == OrderStatusCode.Codes.Approver || a.StatusId == OrderStatusCode.Codes.AccountManager || a.StatusId == OrderStatusCode.Codes.Purchaser || a.StatusId == OrderStatusCode.Codes.Requester || a.StatusId == OrderStatusCode.Codes.ConditionalApprover));

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

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult TotalByPrimaryOrg(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = TotalByWorkgroupViewModel.Create(startDate, endDate, false);

            if (startDate == null || endDate == null)
            {
                Message = "Select a start date and an end date then click apply to view report.";

                return View(viewModel);
            }

            //Grab all workgroups for this user as well as related primary orgs
            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true).ToList();
            var workgroupIds = allWorkgroups.Select(x => x.Id).ToArray();
            var workgroupsWithPrimaryOrgs = _repositoryFactory.WorkgroupRepository.Queryable.Where(x => workgroupIds.Contains(x.Id))
                              .Select(x => new { Workgroup = x, Primary = x.PrimaryOrganization }).ToList();

            //Get every order that matches these workgroups
            var matchingOrders = _searchService.GetOrdersByWorkgroups(allWorkgroups, startDate.Value, endDate.Value);
            var workgroupCounts = new List<OrderTotals>();

            foreach (var workgroup in allWorkgroups)
            {
                if (workgroup.IsActive && !workgroup.Administrative)
                {
                    var orderTotal = workgroupCounts.FirstOrDefault(a => a.PrimaryOrg == workgroup.PrimaryOrganization.Name);
                    if (orderTotal == null)
                    {
                        orderTotal = new OrderTotals();
                        orderTotal.PrimaryOrg = workgroupsWithPrimaryOrgs.Single(x => x.Workgroup.Id == workgroup.Id).Primary.Name;
                        orderTotal.InitiatedOrders = 0;
                        orderTotal.DeniedOrders = 0;
                        orderTotal.CanceledOrders = 0;
                        orderTotal.CompletedOrders = 0;
                        orderTotal.PendingOrders = 0;

                        workgroupCounts.Add(orderTotal);
                    }
                    
                    orderTotal.InitiatedOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id);
                    orderTotal.DeniedOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Denied);
                    orderTotal.CanceledOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.StatusId == OrderStatusCode.Codes.Cancelled);
                    orderTotal.CompletedOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && (a.StatusId == OrderStatusCode.Codes.Complete || a.StatusId == OrderStatusCode.Codes.CompleteNotUploadedKfs));
                    orderTotal.PendingOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && (a.StatusId == OrderStatusCode.Codes.Approver || a.StatusId == OrderStatusCode.Codes.AccountManager || a.StatusId == OrderStatusCode.Codes.Purchaser || a.StatusId == OrderStatusCode.Codes.Requester || a.StatusId == OrderStatusCode.Codes.ConditionalApprover));
                    
                }
            }

            viewModel.WorkgroupCounts = workgroupCounts.OrderBy(a => a.PrimaryOrg);

            var columnPreferences =
                _repositoryFactory.ColumnPreferencesRepository.GetNullableById(CurrentUser.Identity.Name) ??
                new ColumnPreferences(CurrentUser.Identity.Name);
            ViewBag.DataTablesPageSize = columnPreferences.DisplayRows;

            return View(viewModel);
        }

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult TotalByVendor(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = TotalByWorkgroupViewModel.Create(startDate, endDate, false);

            if (startDate == null || endDate == null)
            {
                Message = "Select a start date and an end date then click apply to view report.";

                return View(viewModel);
            }

            //Grab all workgroups for this user
            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true).ToList();
            
            //Get every order that matches these workgroups
            var matchingOrders = _searchService.GetOrdersByWorkgroups(allWorkgroups, startDate.Value, endDate.Value);
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

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult ProcessingTime(int? workgroupId = null, DateTime? month = null, bool? onlyShowReRouted = null)
        {
            Workgroup workgroup = null;

            if (workgroupId.HasValue)
            {
                workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId.Value);
            }
            if (onlyShowReRouted == null)
            {
                onlyShowReRouted = true;
            }
            var viewModel = ReportProcessingTimeViewModel.Create( _workgroupService, workgroup, onlyShowReRouted.Value);

            if (workgroupId.HasValue && month.HasValue)
            {
                viewModel.GenerateDisplayTable(_searchService,_repositoryFactory, _workgroupService,workgroupId.Value, month.Value);
            }

            return View(viewModel);
        }

        [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
        [AuthorizeWorkgroupAccess]
        public ActionResult ProcessingTimeSummary(int? workgroupId = null, DateTime? startDate = null,
            DateTime? endDate = null, bool? onlyShowCompleted = null)
        {
            Workgroup workgroup = null;

            if (workgroupId.HasValue)
            {
                //workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId.Value);
                workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(14);
            }
            //if (onlyShowCompleted == null)
            //{
            //    onlyShowCompleted = true;
            //}
            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            var workgroups = _workgroupService.LoadAdminWorkgroups().ToList();
            
            var viewModel = new ReportProcessingTimeSummaryViewModel()
            {
                Workgroups = workgroups,
                Workgroup = workgroup,
                OnlyShowCompleted = onlyShowCompleted,
                Columns = _searchService.GetOrderTrackingEntities(workgroups, startDate.Value, endDate.Value, onlyShowCompleted)
            };
            return View(viewModel);
            
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
    }

  
}
