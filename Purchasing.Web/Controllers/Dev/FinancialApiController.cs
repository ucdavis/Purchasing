using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.WS;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the FinancialApi class
    /// </summary>
    public class FinancialApiController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public FinancialApiController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [Authorize]
        public ActionResult Index()
        {
            var service = new FinancialSystemService();

            // standard order, no splits
            var order1 = _repositoryFactory.OrderRepository.GetNullableById(40);
            var result1 = service.SubmitOrder(order1, "mceligot");
            ViewBag.Order1DocNum = result1.DocNumber;
            order1.PoNumber = result1.DocNumber;
            _repositoryFactory.OrderRepository.EnsurePersistent(order1);

            // another standard order, with decimal line items
            var order2 = _repositoryFactory.OrderRepository.GetNullableById(42);
            var result2 = service.SubmitOrder(order2, "mceligot");
            ViewBag.Order2DocNum = result2.DocNumber;
            order2.PoNumber = result2.DocNumber;
            _repositoryFactory.OrderRepository.EnsurePersistent(order2);

            // order level splits
            var order3 = _repositoryFactory.OrderRepository.GetNullableById(43);
            var result3 = service.SubmitOrder(order3, "mceligot");
            ViewBag.Order3DocNum = result3.DocNumber;
            order3.PoNumber = result3.DocNumber;
            _repositoryFactory.OrderRepository.EnsurePersistent(order3);

            // line item splits
            var order4 = _repositoryFactory.OrderRepository.GetNullableById(45);
            var result4 = service.SubmitOrder(order4, "mceligot");
            ViewBag.Order4DocNum = result4.DocNumber;
            order4.PoNumber = result4.DocNumber;
            _repositoryFactory.OrderRepository.EnsurePersistent(order4);

            return View();
        }

    }

	
}

