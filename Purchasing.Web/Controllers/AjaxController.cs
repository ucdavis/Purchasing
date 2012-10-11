using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Purchasing.Web.Services;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for AJAX service calls
    /// </summary>
    [SessionState(SessionStateBehavior.Disabled)] //Disabled so requests can run in parallel
    public class AjaxController : ApplicationController
    {
        private readonly ISearchService _searchService;

        public AjaxController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Search for building
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonNetResult SearchBuilding(string term)
        {
            var results = _searchService.SearchBuildings(term);

            return new JsonNetResult(results.Select(a => new { id = a.Id, label = a.Name }).ToList());
        }

        /// <summary>
        /// Ajax call to search for any commodity codes, match by name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonResult SearchCommodityCodes(string searchTerm)
        {
            var results = _searchService.SearchCommodities(searchTerm).Select(a => new IdAndName(a.Id, a.Name));

            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}
