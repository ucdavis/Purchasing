using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
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

        public ReportController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IReportRepositoryFactory reportRepositoryFactory, IReportService reportService, IWorkgroupService workgroupService)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _reportRepositoryFactory = reportRepositoryFactory;
            _reportService = reportService;
            _workgroupService = workgroupService;
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
        public ActionResult TotalByWorkgroup(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = TotalByWorkgroupViewModel.Create(startDate, endDate);

            if (startDate == null || endDate == null)
            {
                Message = "Select a start date and an end date then click apply to view report.";
                

                return View(viewModel);
            }
            var allWorkgroups = _workgroupService.LoadAdminWorkgroups(true);
            var workgroupCounts = new List<OrderTotals>();
            foreach (var workgroup in allWorkgroups)
            {
                if (workgroup.IsActive && !workgroup.Administrative)
                {
                    var orderTotal = new OrderTotals();
                    orderTotal.WorkgroupName = workgroup.Name;
                    orderTotal.WorkgroupId = workgroup.Id;
                    orderTotal.Administrative = false;
                    orderTotal.PrimaryOrg = workgroup.PrimaryOrganization.Name;
                    orderTotal.InitiatedOrders = _repositoryFactory.OrderRepository.Queryable.Count(a => a.Workgroup == workgroup && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value);
                    orderTotal.DeniedOrders = _repositoryFactory.OrderRepository.Queryable.Count(a => a.Workgroup == workgroup && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && a.StatusCode.Id == OrderStatusCode.Codes.Denied);
                    orderTotal.CanceledOrders = _repositoryFactory.OrderRepository.Queryable.Count(a => a.Workgroup == workgroup && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && a.StatusCode.Id == OrderStatusCode.Codes.Cancelled);
                    orderTotal.CompletedOrders = _repositoryFactory.OrderRepository.Queryable.Count(a => a.Workgroup == workgroup && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && (a.StatusCode.Id == OrderStatusCode.Codes.Complete || a.StatusCode.Id == OrderStatusCode.Codes.CompleteNotUploadedKfs));
                    orderTotal.PendingOrders = _repositoryFactory.OrderRepository.Queryable.Count(a => a.Workgroup == workgroup && a.DateCreated >= startDate.Value && a.DateCreated <= endDate.Value && (a.StatusCode.Id == OrderStatusCode.Codes.Approver || a.StatusCode.Id == OrderStatusCode.Codes.AccountManager || a.StatusCode.Id == OrderStatusCode.Codes.Purchaser || a.StatusCode.Id == OrderStatusCode.Codes.Requester || a.StatusCode.Id == OrderStatusCode.Codes.ConditionalApprover));

                    workgroupCounts.Add(orderTotal);
                }
            }

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

                    foreach (var childWorkgroupId in childWorkgroupIds) //NOTE these child workgroup Id's don't include ones that "do not inherit permissions"
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

            viewModel.WorkgroupCounts = workgroupCounts.OrderBy(a => a.WorkgroupName);

            return View(viewModel);
        }

    }

    public class OrderTotals
    {
        public string WorkgroupName { get; set; }
        public int WorkgroupId { get; set; }
        public bool Administrative { get; set; }
        public string PrimaryOrg { get; set; }
        public int InitiatedOrders { get; set; }
        public int DeniedOrders { get; set; }
        public int CanceledOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
    }

    public class TotalByWorkgroupViewModel
    {
        public IEnumerable<OrderTotals> WorkgroupCounts { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static TotalByWorkgroupViewModel Create(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = new TotalByWorkgroupViewModel { StartDate = startDate, EndDate = endDate};
            viewModel.WorkgroupCounts = new List<OrderTotals>();

            return viewModel;
        }

    }
}
