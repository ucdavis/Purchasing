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
        public FileResult Invoice(int id)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            var invoice = _reportService.GetInvoice(order);

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

    }
}
