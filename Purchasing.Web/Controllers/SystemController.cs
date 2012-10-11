using System.Collections.Generic;
using System.Web.Mvc;
using Purchasing.Core.Domain;
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
                                        {"OrderHistory", _indexService.LastModified(Services.Indexes.OrderHistory)}
                                        ,{"LineItems", _indexService.LastModified(Services.Indexes.LineItems)}
                                        ,{"Comments", _indexService.LastModified(Services.Indexes.Comments)}
                                        ,{"CustomAnswers", _indexService.LastModified(Services.Indexes.CustomAnswers)}
                                    };

            ViewBag.NumRecords = new Dictionary<string, int>
                                     {
                                         {"OrderHistory", _indexService.NumRecords(Services.Indexes.OrderHistory)}
                                         ,{"LineItems", _indexService.NumRecords(Services.Indexes.LineItems)}
                                         ,{"Comments", _indexService.NumRecords(Services.Indexes.Comments)}
                                         ,{"CustomAnswers", _indexService.NumRecords(Services.Indexes.CustomAnswers)}
                                     };

            return View();
        }

        public ActionResult Overwrite(string index)
        {
            switch (index)
            {
                case "OrderHistory":
                    _indexService.CreateHistoricalOrderIndex();
                    Message = index + " Updated";
                    break;
                case "LineItems":
                    _indexService.CreateLineItemsIndex();
                    Message = index + " Updated";
                    break;
                case "Comments":
                    _indexService.CreateCommentsIndex();
                    Message = index + " Updated";
                    break;
                case "CustomAnswers":
                    _indexService.CreateCustomAnswersIndex();
                    Message = index + " Updated";
                    break;
            }

            return RedirectToAction("Indexes");
        }
    }
}