using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Queries;
using Purchasing.Web.Attributes;
using Purchasing.Web.Services;
using Purchasing.Core;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Search class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class SearchController : ApplicationController
    {
        private readonly ISearchService _searchService;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IUserIdentity _userIdentity;

        public SearchController(ISearchService searchService, IQueryRepositoryFactory queryRepositoryFactory, IUserIdentity userIdentity)
        {
            _searchService = searchService;
            _queryRepositoryFactory = queryRepositoryFactory;
            _userIdentity = userIdentity;
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

            var orderIds = _queryRepositoryFactory.AccessRepository.Queryable
                .Where(a => a.AccessUserId == _userIdentity.Current)
                .Select(x=>x.OrderId)
                .Distinct()
                .ToArray();
            
            var model = new SearchResultModel
                            {
                                Query = q,
                                Orders = _searchService.SearchOrders(q, orderIds),
                                LineItems = _searchService.SearchLineItems(q, orderIds),
                                Comments = _searchService.SearchComments(q, orderIds),
                                CustomFields = _searchService.SearchCustomFieldAnswers(q, orderIds)
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
