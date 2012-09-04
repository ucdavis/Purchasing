using System.Collections.Generic;
using System.Web.Mvc;
using Purchasing.Web.Services;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;
using System;

namespace Purchasing.Web.Controllers
{
    [HandleTransactionsManually]
    [Authorize(Roles = Role.Codes.Admin)]
    public class SystemController : SuperController
    {
        private readonly IIndexService _indexService;

        public SystemController(IIndexService indexService)
        {
            _indexService = indexService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Indexes()
        {
            ViewBag.ModifiedDates = new Dictionary<string, DateTime>
                                        {
                                            {"OrderHistory", _indexService.LastModified(Services.Indexes.OrderHistory)},
                                            {"Access", _indexService.LastModified(Services.Indexes.Access)}
                                        };

            ViewBag.NumRecords = new Dictionary<string, int>
                                     {
                                         {"OrderHistory", _indexService.NumRecords(Services.Indexes.OrderHistory)},
                                         {"Access", _indexService.NumRecords(Services.Indexes.Access)}
                                     };

            return View();
        }

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