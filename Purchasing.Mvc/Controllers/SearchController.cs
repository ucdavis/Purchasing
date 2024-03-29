﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Controllers;
using Serilog;

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

            if (Regex.IsMatch(q.Trim().ToUpper(), "^[A-Z,0-9]{4,4}-[A-Z,0-9]{6,7}$"))
            {
                if (Repository.OfType<Order>().Queryable.Any(a => a.RequestNumber == q.Trim().ToUpper()))
                {
                    return this.RedirectToAction(nameof(OrderController.Lookup), typeof(OrderController).ControllerName(), new { id = q.Trim().ToUpper() });
                }
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
                Log.Error(ex, "Error searching for {query}", q);
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
