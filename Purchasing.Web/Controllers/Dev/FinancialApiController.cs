using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
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

            var order = _repositoryFactory.OrderRepository.Queryable.Where(a => a.StatusCode.IsComplete).FirstOrDefault();

            if (order != null)
            {
                service.SubmitOrder(order, CurrentUser.Identity.Name);
            }

            return View();
        }

    }

	
}
