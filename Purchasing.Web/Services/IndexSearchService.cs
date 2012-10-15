using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Version = Lucene.Net.Util.Version;
using UCDArch.Core.Utils;
using Purchasing.Web.Utility;

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
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.OrderResult.SearchableFields);

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
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.LineResult.SearchableFields);

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
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.CustomFieldResult.SearchableFields);

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
            var searcher = _indexService.GetIndexSearcherFor(Indexes.Comments);
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.CommentResult.SearchableFields);

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

        public IList<IdAndName> SearchCommodities(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Commodities, searchTerm);
        }

        public IList<IdAndName> SearchVendors(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Vendors, searchTerm);
        }

        public IList<IdAndName> SearchAccounts(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Accounts, searchTerm);
        }

        public IList<IdAndName> SearchBuildings(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Buildings, searchTerm);
        }

        private IList<IdAndName> SearchLookupIndex(Indexes index, string searchTerm, int topN = 20)
        {
            var searcher = _indexService.GetIndexSearcherFor(index);
            
            //Default to standard analyzer but analyze the id field using keyword anaylzer since it shouldn't be tokenized
            var analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(Version.LUCENE_29));
            analyzer.AddAnalyzer("id", new KeywordAnalyzer());

            var termsQuery =
                new MultiFieldQueryParser(Version.LUCENE_29, new[] { "id", "name" }, analyzer).Parse(searchTerm);
            var results = searcher.Search(termsQuery, topN).ScoreDocs;

            analyzer.Close();

            var entities = results
                .Select(scoreDoc => searcher.Doc(scoreDoc.doc))
                .Select(doc => new IdAndName(doc.Get("id"), doc.Get("name"))).ToList();

            searcher.Close();
            searcher.Dispose();

            return entities;
        }

        /// <summary>
        /// Searches an index by the searchTerm and across searchableFields, filtering the results to only within the given orderIds
        /// </summary>
        /// <returns>ScoreDoc hits</returns>
        private IEnumerable<ScoreDoc> SearchIndex(IndexSearcher searcher, IEnumerable<int> filteredOrderIds, string searchTerm, string[] searchableFields, int topN = 20)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            Query accessQuery = new QueryParser(Version.LUCENE_29, "orderid", analyzer).Parse(string.Join(" ", filteredOrderIds));
            var termsQuery =
                new MultiFieldQueryParser(Version.LUCENE_29, searchableFields, analyzer).Parse(searchTerm);
            var results = searcher.Search(termsQuery, new CachingWrapperFilter(new QueryWrapperFilter(accessQuery)), topN).ScoreDocs;

            analyzer.Close();

            return results;
        }
    }
}