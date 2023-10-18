using System;
using System.Linq;
using Purchasing.Core.Services;
using Purchasing.Mvc.Utility;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using IdAndName = Purchasing.Core.Services.IdAndName;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for AJAX service calls
    /// </summary>
    public class AjaxController : ApplicationController
    {
        private readonly ISearchService _searchService;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public AjaxController(ISearchService searchService, IAggieEnterpriseService aggieEnterpriseService)
        {
            _searchService = searchService;
            _aggieEnterpriseService = aggieEnterpriseService;
        }

        /// <summary>
        /// Search for building
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [Obsolete("We don't use building codes anymore")]
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
        public  JsonResult SearchCommodityCodes(string searchTerm)
        {
            //var results = _searchService.SearchCommodities(searchTerm).Select(a => new IdAndName(a.Id, a.Name));
            var results = _aggieEnterpriseService.SearchCommodities(searchTerm).GetAwaiter().GetResult().Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return Json(results);
        }
    }
}
