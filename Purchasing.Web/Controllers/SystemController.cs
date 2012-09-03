using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Purchasing.Web.Services;
using UCDArch.Web.Attributes;
using UCDArch.Web.Controller;

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
            return Content("Order history last modified " + _indexService.LastModified(Indexes.OrderHistory).ToLongDateString());
        }
    }
}