using System.Collections.Generic;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;

namespace Purchasing.Core.Repositories
{
    public class IndexSearchRepository : ISearchRepository
    {
        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, string user)
        {
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