using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the AlanTest class
    /// </summary>
    public class AlanTestController : ApplicationController
    {
        private readonly IOrderAccessService _orderAccessService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Approval> _approvalRepository;

        public AlanTestController(IOrderAccessService orderAccessService, IRepository<Order> orderRepository, IRepository<Approval> approvalRepository )
        {
            _orderAccessService = orderAccessService;
            _orderRepository = orderRepository;
            _approvalRepository = approvalRepository;
        }

        public ActionResult Index()
        {
            var viewModel = DashboardViewModel.Create(Repository);
            return View(viewModel);
        }

        public class DashboardViewModel
        {
            public static DashboardViewModel Create(IRepository repository)
            {
                Check.Require(repository != null, "Repository is required.");

                var viewModel = new DashboardViewModel();

                return viewModel;
            }
        }

    }
}
