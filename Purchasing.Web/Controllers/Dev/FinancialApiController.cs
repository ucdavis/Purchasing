using System;
using System.Collections.Generic;
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
            var service2 = new FinancialRoleSystemService();

            var order = _repositoryFactory.OrderRepository.Queryable.Where(a => a.StatusCode.IsComplete).FirstOrDefault();

            if (order != null)
            {
                service.SubmitOrder(order, CurrentUser.Identity.Name);
            }

            //service.GetOrderStatus("12345678");

            //service.GetDocumentUrl("12345678");

            //var result = service2.GetAccountInfo(new AccountInfo() {Chart = "3", Number = "CRU9033"});

            //var accts = new List<AccountInfo>();
            //accts.Add(new AccountInfo() { Chart = "3", Number = "CRU9033" });
            //accts.Add(new AccountInfo() { Chart = "3", Number = "CRUCLST" });
            //accts.Add(new AccountInfo() { Chart = "3", Number = "FAKE" });
            //accts.Add(new AccountInfo() { Chart = "3", Number = "CRUGRAD" });
            //var result2 = service2.GetAccountInfos(accts);

            //var result3 = service2.IsFiscalOfficer(new AccountInfo() {Chart = "3", Number = "CRU9033"}, "anlai");

            //var result4 = service2.IsFiscalOfficer(new AccountInfo() { Chart = "3", Number = "CRU9033" }, "mceligot");

            return View();
        }

    }

	
}

