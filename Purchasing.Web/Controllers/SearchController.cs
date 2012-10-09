using System.Collections.Generic;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Attributes;
using Purchasing.Web.Services;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Search class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class SearchController : ApplicationController
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
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
            var saveSearchTerm = q;
            q = q.Replace("'", "''");

            var model = new SearchResultModel
                            {
                                Query = saveSearchTerm,
                                Orders = _searchService.SearchOrders(q, CurrentUser.Identity.Name),
                                LineItems = _searchService.SearchLineItems(q, CurrentUser.Identity.Name),
                                Comments = _searchService.SearchComments(q, CurrentUser.Identity.Name),
                                CustomFields = _searchService.SearchCustomFieldAnswers(q, CurrentUser.Identity.Name)
                            };

            return View(model);
        }
    }

    public class SearchResultModel
    {
        public IList<SearchResults.OrderResult> Orders { get; set; }
        public IList<SearchResults.CommentResult> Comments { get; set; }
        public IList<SearchResults.LineResult> LineItems { get; set; }
        public IList<SearchResults.CustomFieldResult> CustomFields { get; set; }
        public string Query { get; set; }
        public int ResultCount { get { return (Orders.Count + LineItems.Count + Comments.Count + CustomFields.Count); } }

    }
}
