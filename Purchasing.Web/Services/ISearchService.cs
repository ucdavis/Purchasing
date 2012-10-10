using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using UCDArch.Data.NHibernate;

namespace Purchasing.Web.Services
{
    /// <summary>
    /// Service for full text search queries. Each query passes a list of OrderIds that should be searched
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Searches orders across the following fields: [Justification], [RequestNumber], [DeliverTo], [DeliverToEmail]
        /// </summary>
        IList<SearchResults.OrderResult> SearchOrders(string searchTerm, int[] allowedIds);
        
        /// <summary>
        /// Searches line items across the following fileds: [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes]
        /// </summary>
        IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds);

        /// <summary>
        /// Searches custom field answers across the following fields: [Answer]
        /// </summary>
        IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds);

        /// <summary>
        /// Searches comments across the following fields: [text]
        /// </summary>
        IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds);

        /// <summary>
        /// Searches commodities
        /// </summary>
        IList<Commodity> SearchCommodities(string searchTerm);

        /// <summary>
        /// Searches vendors
        /// </summary>
        IList<Vendor> SearchVendors(string searchTerm);

        /// <summary>
        /// Searches accounts
        /// </summary>
        IList<Account> SearchAccounts(string searchTerm);

        /// <summary>
        /// Searches buildings
        /// </summary>
        IList<Building> SearchBuildings(string searchTerm);
    }

    /*
    public class SearchService : ISearchService
    {
        public ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        #region ISearchService Members

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, string user, int[] allowedIds = null)
        {
            return PerformQuery<SearchResults.OrderResult>(searchTerm, "Order", user);
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, string user, int[] allowedIds = null)
        {
            return PerformQuery<SearchResults.LineResult>(searchTerm, "Line", user);
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, string user, int[] allowedIds = null)
        {
            return PerformQuery<SearchResults.CustomFieldResult>(searchTerm, "CustomField", user);
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, string user, int[] allowedIds = null)
        {
            return PerformQuery<SearchResults.CommentResult>(searchTerm, "Comment", user);
        }

        /// <summary>
        /// Searches commodities
        /// </summary>
        public IList<Commodity> SearchCommodities(string searchTerm)
        {
            return PerformQuery<Commodity>(searchTerm, "Commodities");
        }

        /// <summary>
        /// Searches buildings
        /// </summary>
        public IList<Building> SearchBuildings(string searchTerm)
        {
            return PerformQuery<Building>(searchTerm, "Building");
        }

        /// <summary>
        /// Searches vendors
        /// </summary>
        public IList<Vendor> SearchVendors(string searchTerm)
        {
            return PerformQuery<Vendor>(searchTerm, "Vendor");
        }

        /// <summary>
        /// Searches accounts
        /// </summary>
        public IList<Account> SearchAccounts(string searchTerm)
        {
            return PerformQuery<Account>(searchTerm, "Account");
        }

        #endregion

        private IList<T> PerformQuery<T>(string query, string queryType, string user = null)
        {
            var queryParams = new List<string> {"@User", "@ContainsSearchCondition"};

            if (string.IsNullOrWhiteSpace(user))
            {
                queryParams.Remove("@User");
            }

            IQuery q = Session.CreateSQLQuery(
                string.Format(
                    @"DECLARE @User varchar(255) = '{0}';
                    DECLARE @ContainsSearchCondition varchar(255) = '{1}';
                    SELECT * from dbo.udf_Get{2}Results({3})",
                    user, CleanSearchTerm(query), queryType, string.Join(",", queryParams)))
                .SetResultTransformer(Transformers.AliasToBean<T>());

            return q.List<T>();
        }

        private string CleanSearchTerm(string term)
        {
            return string.IsNullOrWhiteSpace(term) ? string.Empty : term.ToLower().Trim();
        }
    }
     */
}