using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.Attributes;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;

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

        public ReportController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IReportRepositoryFactory reportRepositoryFactory, IReportService reportService)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _reportRepositoryFactory = reportRepositoryFactory;
            _reportService = reportService;
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
        public ActionResult Workload(int? workgroupId)
        {
            var viewModel = ReportWorkloadViewModel.Create(_repositoryFactory, _queryRepositoryFactory, CurrentUser.Identity.Name);

            if (workgroupId.HasValue)
            {
                viewModel.GenerateDisplayTable(_reportRepositoryFactory, workgroupId.Value);
            }
            
            return View(viewModel);
        }
    }
}
