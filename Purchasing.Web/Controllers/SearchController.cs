using System.Collections.Generic;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core.Repositories;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Search class
    /// </summary>
    [Authorize]
    public class SearchController : ApplicationController
    {
        private readonly ISearchRepository _searchRepository;

        public SearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        /// <summary>
        /// Will searches and display results depending on what matches the query "q"
        /// </summary>
        /// <param name="q">Search query to search on</param>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index");
            }

            var model = new SearchResultModel {Orders = _searchRepository.SearchOrders(q)};

            return View(model);
        }
    }

    public class SearchResultModel
    {
        public IList<SearchResults.OrderResult> Orders { get; set; }
        public string Query { get; set; }
        public int ResultCount {get { return Orders.Count; }}
    }
}
