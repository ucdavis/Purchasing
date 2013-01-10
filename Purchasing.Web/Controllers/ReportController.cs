using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Attributes;
using Purchasing.Web.Models;
using Purchasing.Web.Services;

namespace Purchasing.Web.Controllers
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
        private readonly IIndexService _indexService;

        public ReportController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IReportRepositoryFactory reportRepositoryFactory, IReportService reportService, IWorkgroupService workgroupService, IIndexService indexService)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _reportRepositoryFactory = reportRepositoryFactory;
            _reportService = reportService;
            _workgroupService = workgroupService;
            _indexService = indexService;
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
            var matchingOrders = GetOrdersByWorkgroups(allWorkgroups);
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
                    orderTotal.InitiatedOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value);
                    orderTotal.DeniedOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && a.StatusId == OrderStatusCode.Codes.Denied);
                    orderTotal.CanceledOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && a.StatusId == OrderStatusCode.Codes.Cancelled);
                    orderTotal.CompletedOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && (a.StatusId == OrderStatusCode.Codes.Complete || a.StatusId == OrderStatusCode.Codes.CompleteNotUploadedKfs));
                    orderTotal.PendingOrders = matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && (a.StatusId == OrderStatusCode.Codes.Approver || a.StatusId == OrderStatusCode.Codes.AccountManager || a.StatusId == OrderStatusCode.Codes.Purchaser || a.StatusId == OrderStatusCode.Codes.Requester || a.StatusId == OrderStatusCode.Codes.ConditionalApprover));

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
            var matchingOrders = GetOrdersByWorkgroups(allWorkgroups);
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
                    
                    orderTotal.InitiatedOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value);
                    orderTotal.DeniedOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && a.StatusId == OrderStatusCode.Codes.Denied);
                    orderTotal.CanceledOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && a.StatusId == OrderStatusCode.Codes.Cancelled);
                    orderTotal.CompletedOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && (a.StatusId == OrderStatusCode.Codes.Complete || a.StatusId == OrderStatusCode.Codes.CompleteNotUploadedKfs));
                    orderTotal.PendingOrders += matchingOrders.AsQueryable().Count(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && (a.StatusId == OrderStatusCode.Codes.Approver || a.StatusId == OrderStatusCode.Codes.AccountManager || a.StatusId == OrderStatusCode.Codes.Purchaser || a.StatusId == OrderStatusCode.Codes.Requester || a.StatusId == OrderStatusCode.Codes.ConditionalApprover));
                    
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
            var matchingOrders = GetOrdersByWorkgroups(allWorkgroups);
            var workgroupCounts = new List<OrderTotals>();

            foreach (var workgroup in allWorkgroups)
            {
                if (workgroup.IsActive && !workgroup.Administrative)
                {
                    var orders = matchingOrders.AsQueryable().Where(a => a.WorkgroupId == workgroup.Id && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value);

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

        private List<OrderHistory> GetOrdersByWorkgroups(IEnumerable<Workgroup> workgroups)
        {
            var workgroupIds = workgroups.Select(x => x.Id).Distinct().ToArray();

            var searcher = _indexService.GetIndexSearcherFor(Indexes.OrderHistory);

            //Search for all orders within the given workgroups
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            Query query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "workgroupid", analyzer).Parse(string.Join(" ", workgroupIds));

            //Need to return all matching orders
            var docs = searcher.Search(query, int.MaxValue).ScoreDocs;
            var orderHistory = new List<OrderHistory>();

            foreach (var scoredoc in docs)
            {
                var doc = searcher.Doc(scoredoc.doc);

                var history = new OrderHistory();

                foreach (var prop in history.GetType().GetProperties())
                {
                    if (!string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        prop.SetValue(history, Convert.ChangeType(doc.Get(prop.Name.ToLower()), prop.PropertyType), null);
                    }
                }

                orderHistory.Add(history);
            }

            analyzer.Close();
            searcher.Close();
            searcher.Dispose();

            return orderHistory;
        }
    }
}
