using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Web.Attributes;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Report class
    /// </summary>
    public class ReportController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IReportService _reportService;

        public ReportController(IRepositoryFactory repositoryFactory, IReportService reportService)
        {
            _repositoryFactory = repositoryFactory;
            _reportService = reportService;
        }

        [AuthorizeReadOrEditOrder]
        public FileResult Invoice(int id)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            var invoice = _reportService.GetInvoice(order);

            return File(invoice, "application/pdf");
        }

    }
}
