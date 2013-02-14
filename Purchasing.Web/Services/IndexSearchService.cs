using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Purchasing.Core.Queries;
using Purchasing.Web.Utility;
using Version = Lucene.Net.Util.Version;

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
            try
            {
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
                        DateCreated = DateTime.Parse(doc.Get("datecreated")),
                        PoNumber = doc.Get("ponumber"),
                        ReferenceNumber = doc.Get("referencenumber"),
                        Approver = doc.Get("approver"),
                        AccountManager = doc.Get("accountmanager"),
                        Purchaser = doc.Get("purchaser")
                    }).ToList();


                return orderResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }

        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.LineItems);
            try
            {
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

                return lineResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }

        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.CustomAnswers);
            try
            {
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


                return customFieldResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.Comments);
            try
            {
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

                return commentResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }

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
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<IdAndName>();
            }
            var searcher = _indexService.GetIndexSearcherFor(index);
            
            //Default to standard analyzer-- id field is tokenized into searchid non-stored field
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            try
            {
                var termsQuery = new MultiFieldQueryParser(Version.LUCENE_29, new[] { "searchid", "name" }, analyzer).Parse(searchTerm);
                var results = searcher.Search(termsQuery, topN).ScoreDocs;

                var entities = results
                    .Select(scoreDoc => searcher.Doc(scoreDoc.doc))
                    .Select(doc => new IdAndName(doc.Get("id"), doc.Get("name"))).ToList();

                return entities;
            }
            catch (ParseException)
            {
                return new List<IdAndName>();
            }
            finally
            {
                    analyzer.Close();
                    searcher.Close();
                    searcher.Dispose();
            }
       


            
        }

        /// <summary>
        /// Searches an index by the searchTerm and across searchableFields, filtering the results to only within the given orderIds
        /// </summary>
        /// <returns>ScoreDoc hits</returns>
        private IEnumerable<ScoreDoc> SearchIndex(IndexSearcher searcher, IEnumerable<int> filteredOrderIds, string searchTerm, string[] searchableFields, int topN = 20)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            try
            {
                Query accessQuery = new QueryParser(Version.LUCENE_29, "orderid", analyzer).Parse(string.Join(" ", filteredOrderIds));
                var termsQuery =
                    new MultiFieldQueryParser(Version.LUCENE_29, searchableFields, analyzer).Parse(searchTerm);
                var results = searcher.Search(termsQuery, new CachingWrapperFilter(new QueryWrapperFilter(accessQuery)), topN).ScoreDocs;                

                return results;
            }
            catch (ParseException)
            {
                return null;
            }
            finally
            {
                analyzer.Close();
            }

        }
    }
}