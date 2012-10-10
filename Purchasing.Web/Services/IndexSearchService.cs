using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Version = Lucene.Net.Util.Version;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    /// <summary>
    /// Searches using Lucene indexes
    /// TODO: MUST filter orders by user
    /// </summary>
    public class IndexSearchService : ISearchService
    {
        private readonly IIndexService _indexService;

        public IndexSearchService(IIndexService indexService)
        {
            _indexService = indexService;
        }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.OrderHistory);
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, OrderHistory.SearchableFields);

            var orderResults = results
                .Select(scoreDoc => searcher.Doc(scoreDoc.doc))
                .Select(doc => new SearchResults.OrderResult
                                   {
                                       Id = int.Parse(doc.Get("orderid")),
                                       Justification = doc.Get("justification"),
                                       CreatedBy = doc.Get("createdby"),
                                       DeliverTo = doc.Get("shipto"),
                                       DeliverToEmail = doc.Get("shiptoemail"),
                                       RequestNumber = doc.Get("requestnumber"),
                                       DateCreated = DateTime.Parse(doc.Get("datecreated"))
                                   }).ToList();

            searcher.Close();
            searcher.Dispose();

            return orderResults;
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.LineItems);
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm,
                                                        new[]
                                                            {
                                                                "description", "url", "notes", "catalognumber",
                                                                "commodityid", "receivednotes"
                                                            });

            var lineResults = results
                .Select(scoreDoc => searcher.Doc(scoreDoc.doc))
                .Select(doc => new SearchResults.LineResult
                {
                    OrderId = int.Parse(doc.Get("orderid")),
                    Description = doc.Get("description"),
                    Url = doc.Get("url"),
                    Notes = doc.Get("notes"),
                    CatalogNumber = doc.Get("catalognumber"),
                    CommodityId = doc.Get("commodityid"),
                    ReceivedNotes = doc.Get("receivednotes"),
                    RequestNumber = doc.Get("requestnumber"),
                    Quantity = decimal.Parse(doc.Get("quantity")),
                    Unit = doc.Get("unit")
                }).ToList();

            searcher.Close();
            searcher.Dispose();

            return lineResults;
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.CustomAnswers);
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, new[] {"answer"});

            var customFieldResults = results
                .Select(scoreDoc => searcher.Doc(scoreDoc.doc))
                .Select(doc => new SearchResults.CustomFieldResult
                {
                    OrderId = int.Parse(doc.Get("orderid")),
                    RequestNumber = doc.Get("requestnumber"),
                    Answer = doc.Get("answer"),
                    Question = doc.Get("question")
                }).ToList();

            searcher.Close();
            searcher.Dispose();

            return customFieldResults;
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.CustomAnswers);
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, new[] { "answer" });

            var commentResults = results
                .Select(scoreDoc => searcher.Doc(scoreDoc.doc))
                .Select(doc => new SearchResults.CommentResult()
                {
                    OrderId = int.Parse(doc.Get("orderid")),
                    RequestNumber = doc.Get("requestnumber"),
                    Text = doc.Get("text"),
                    CreatedBy = doc.Get("createdby"),
                    DateCreated = DateTime.Parse(doc.Get("datecreated"))
                }).ToList();

            searcher.Close();
            searcher.Dispose();

            return commentResults;
        }

        public IList<Commodity> SearchCommodities(string searchTerm)
        {
            throw new System.NotImplementedException();
        }

        public IList<Vendor> SearchVendors(string searchTerm)
        {
            throw new System.NotImplementedException();
        }

        public IList<Account> SearchAccounts(string searchTerm)
        {
            throw new System.NotImplementedException();
        }

        public IList<Building> SearchBuildings(string searchTerm)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Searches an index by the searchTerm and across searchableFields, filtering the results to only within the given orderIds
        /// </summary>
        /// <returns>ScoreDoc hits</returns>
        private IEnumerable<ScoreDoc> SearchIndex(IndexSearcher searcher, IEnumerable<int> filteredOrderIds, string searchTerm, string[] searchableFields)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            Query accessQuery = new QueryParser(Version.LUCENE_29, "orderid", analyzer).Parse(string.Join(" ", filteredOrderIds));
            var termsQuery =
                new MultiFieldQueryParser(Version.LUCENE_29, searchableFields, analyzer).Parse(searchTerm);
            var results = searcher.Search(termsQuery, new CachingWrapperFilter(new QueryWrapperFilter(accessQuery)), 20).ScoreDocs;

            analyzer.Close();

            return results;
        }
    }
}