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

        /// Searches orders across the following fields: [Justification], [RequestNumber], [DeliverTo], [DeliverToEmail]
        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, string user)
        {
            //TODO: filter by user somehow.... maybe order history can have all user names?
            var distinctOrderIds = new[] {747, 1149, 528, 151, 354, 123, 222, 365, 33, 98};

            var searcher = _indexService.GetIndexSearcherFor(Indexes.OrderHistory);
            IEnumerable<ScoreDoc> results = SearchIndex(searcher, distinctOrderIds, searchTerm, OrderHistory.SearchableFields);

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

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, string user)
        {
            throw new System.NotImplementedException();
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, string user)
        {
            throw new System.NotImplementedException();
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, string user)
        {
            throw new System.NotImplementedException();
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
    }
}