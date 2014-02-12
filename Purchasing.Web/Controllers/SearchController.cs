using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Purchasing.Core.Queries;
using Purchasing.Web.Attributes;
using Purchasing.Web.Services;
using Purchasing.Core;
using System;
using MvcContrib;
using SendGridMail;
using SendGridMail.Transport;
using UCDArch.Core.Utils;

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

            if (Regex.IsMatch(q, "^[A-Z]{4,4}-[A-Z,0-9]{7,7}$"))
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
                var sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"];
                var sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"];
                SendSingleEmail(string.Format("Search term: '{0}' ====== User {1} ======= {2} ====== {3}", q, _userIdentity.Current, ex.Message, ex.InnerException.ToString()), sendGridUserName, sendGridPassword);
                model = new SearchResultModel {Query = q};
            }

            return View(model);
        }

        private void SendSingleEmail(string body, string sendGridUserName, string sendGridPassword)
        {
            Check.Require(!string.IsNullOrWhiteSpace(sendGridUserName));
            Check.Require(!string.IsNullOrWhiteSpace(sendGridPassword));

            var sgMessage = SendGrid.GenerateInstance();
            sgMessage.From = new MailAddress("opp-Exception@ucdavis.edu", "OPP No Reply");
            sgMessage.Subject = "Lucene Exception";
            sgMessage.AddTo("opp-tech-request@ucdavis.edu");
            sgMessage.Html = body;

            var transport = SMTP.GenerateInstance(new NetworkCredential(sendGridUserName, sendGridPassword));
            transport.Deliver(sgMessage);
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
