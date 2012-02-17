using System.Web.Mvc;
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
        public ActionResult Index(string q)
        {
            //if (string.IsNullOrWhiteSpace(q)) { }
            return View();
        }

    }
	
}
