using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;

namespace Purchasing.Mvc.Controllers
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
        private readonly IAccessQueryService _accessQueryService;

        public SearchController(ISearchService searchService, IQueryRepositoryFactory queryRepositoryFactory, IUserIdentity userIdentity, IAccessQueryService accessQueryService)
        {
            _searchService = searchService;
            _queryRepositoryFactory = queryRepositoryFactory;
            _userIdentity = userIdentity;
            _accessQueryService = accessQueryService;
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

            if (Regex.IsMatch(q, "^[A-Z,0-9]{4,4}-[A-Z,0-9]{7,7}$"))
            {
                return this.RedirectToAction<OrderController>(a => a.Lookup(q));
            }
 

            var orderIds = _accessQueryService.GetOrderAccess(_userIdentity.Current)
                .Select(x=>x.OrderId)
                .Distinct()
                .ToArray();

            SearchResultModel model;
            
            try
            {
                model = new SearchResultModel
                {
                    Query = q,
                    Orders = _searchService.SearchOrders(q, orderIds),
                    LineItems = _searchService.SearchLineItems(q, orderIds),
                    Comments = _searchService.SearchComments(q, orderIds),
                    CustomFields = _searchService.SearchCustomFieldAnswers(q, orderIds)
                };
            }
            catch(Exception ex) //If the search fails, return no results
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                model = new SearchResultModel {Query = q};
            }

            return View(model);
        }

    }

    public class SearchResultModel
    {
        public SearchResultModel()
        {
            Orders = new List<SearchResults.OrderResult>();
            Comments = new List<SearchResults.CommentResult>();
            LineItems = new List<SearchResults.LineResult>();
            CustomFields = new List<SearchResults.CustomFieldResult>();
        }

        public IList<SearchResults.OrderResult> Orders { get; set; }
        public IList<SearchResults.CommentResult> Comments { get; set; }
        public IList<SearchResults.LineResult> LineItems { get; set; }
        public IList<SearchResults.CustomFieldResult> CustomFields { get; set; }
        public string Query { get; set; }
        public int ResultCount { get { return (Orders.Count + LineItems.Count + Comments.Count + CustomFields.Count); } }

    }
}
