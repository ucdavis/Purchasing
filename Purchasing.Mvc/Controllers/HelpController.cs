using System;
using Purchasing.Core.Helpers;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Purchasing.Core.Services;
using System.Threading.Tasks;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// help controller
    /// </summary>
    /// <remarks>
    /// </remarks>
    [HandleTransactionsManually]
    public class HelpController : ApplicationController
    {
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public HelpController(IAggieEnterpriseService aggieEnterpriseService) 
        {
            _aggieEnterpriseService = aggieEnterpriseService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Kfs(string id)
        {
            try
            {
                var rtValue = await _aggieEnterpriseService.ConvertKfsAccount(id, false);

                return Content(rtValue);
            }
            catch (Exception)
            {
                return Content("Not found or bad format. ");
            }
        }
    }
}