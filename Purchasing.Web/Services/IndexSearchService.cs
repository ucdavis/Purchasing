using System.Collections.Generic;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Lucene.Net.Util;

namespace Purchasing.Web.Services
{
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
            var searcher = _indexService.GetIndexSearcherFor(Indexes.OrderHistory);
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            var query = new MultiFieldQueryParser(Version.LUCENE_29, new string[] {"RequestNumber", "ShipTo", "LineItems"}, analyzer);
            //Query query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "orderid", analyzer).Parse(searchTerm);
            

            throw new System.NotImplementedException();
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