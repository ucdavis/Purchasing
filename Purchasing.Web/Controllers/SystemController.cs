using System.Collections.Generic;
using System.Web.Mvc;
using Purchasing.Web.Services;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;
using System;

namespace Purchasing.Web.Controllers
{
    public class SystemController : SuperController
    {
        private readonly IIndexService _indexService;

        public SystemController(IIndexService indexService)
        {
            _indexService = indexService;
        }

        [HandleTransactionsManually]
        public ActionResult Index()
        {
            return View();
        }

        [HandleTransactionsManually]
        public ActionResult Indexes()
        {
            var modifiedDates = new Dictionary<string, System.DateTime>();
            modifiedDates["OrderHistory"] = _indexService.LastModified(Services.Indexes.OrderHistory);
            modifiedDates["Access"] = _indexService.LastModified(Services.Indexes.Access);
            
            return View(modifiedDates);
        }

        [HandleTransactionsManually]
        public ActionResult Overwrite(string index)
        {
            switch (index)
            {
                case "OrderHistory":
                    _indexService.CreateHistoricalOrderIndex();
                    Message = "Historical Order Index Updated";
                    break;
                case "Access":
                    _indexService.CreateAccessIndex();
                    Message = "Access Index Updated";
                    break;
            }

            return RedirectToAction("Indexes");
        }
    }
}