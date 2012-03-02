using System;
using System.Collections.Generic;
using NHibernate.Transform;
using Purchasing.Core.Queries;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Purchasing.Core.Domain;
using NHibernate;

namespace Purchasing.Core.Repositories
{
    /// <summary>
    /// Repository for full text search queries
    /// </summary>
    public interface ISearchRepository
    {
        IList<SearchResults.OrderResult> SearchOrders(string searchTerm, string user);
        IList<SearchResults.LineResult> SearchLineItems(string searchTerm, string user);
        IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, string user);
        IList<SearchResults.CommentResult> SearchComments(string searchTerm, string user);

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
    }

    public class SearchRepository : ISearchRepository
    {
        public ISession Session { get { return NHibernateSessionManager.Instance.GetSession(); } }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, string user)
        {
            const string queryFields = "(RES.[Justification], RES.[RequestNumber], RES.[DeliverTo], RES.[DeliverToEmail])";
            return PerformQuery<SearchResults.OrderResult>(user, searchTerm, queryFields, "Order", queryId: "Id");
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, string user)
        {
            const string queryFields = "(RES.[Description], RES.[Url], RES.[Notes], RES.[CatalogNumber], RES.[CommodityId])";
            return PerformQuery<SearchResults.LineResult>(user, searchTerm, queryFields, "Line");
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, string user)
        {
            const string queryFields = "(RES.[Answer])";
            return PerformQuery<SearchResults.CustomFieldResult>(user, searchTerm, queryFields, "CustomField");
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, string user)
        {
            const string queryFields = "(RES.[text])";
            return PerformQuery<SearchResults.CommentResult>(user, searchTerm, queryFields, "Comment");
        }

        private IList<T> PerformQuery<T>(string user, string query, string queryFields, string queryType, string queryId = "OrderId")
        {
            var q = Session.CreateSQLQuery(string.Format(@"SELECT RES.* FROM [PrePurchasing].[dbo].[v{0}Results] RES
                INNER JOIN [PrePurchasing].[dbo].[vAccess] A 
                ON RES.[{1}] = A.[OrderId] 
                WHERE FREETEXT({2}, :query) 
                AND A.[AccessUserId] = :user AND A.[isadmin] = 0", queryType, queryId, queryFields))
                .SetString("query", query)
                .SetString("user", user)
                .SetResultTransformer(Transformers.AliasToBean<T>());

            return q.List<T>();
        }

        private IList<T> PerformUdfQuery<T>(string query, string queryType, string user = null)
        {
            var q = Session.CreateSQLQuery(
                string.Format(
                    @"DECLARE @ContainsSearchCondition varchar(255) = '{0}';
                    SELECT * from dbo.udf_Get{1}Results(@ContainsSearchCondition)",
                    query, queryType))
                .SetResultTransformer(Transformers.AliasToBean<T>());

            return q.List<T>();
        } 

        /// <summary>
        /// Searches commodities
        /// </summary>
        public IList<Commodity> SearchCommodities(string searchTerm)
        {
            return PerformUdfQuery<Commodity>(searchTerm, "Commodities");
        }

        /// <summary>
        /// Searches vendors
        /// </summary>
        public IList<Vendor> SearchVendors(string searchTerm)
        {
            return PerformUdfQuery<Vendor>(searchTerm, "Vendor");
        }

        /// <summary>
        /// Searches accounts
        /// </summary>
        public IList<Account> SearchAccounts(string searchTerm)
        {
            return PerformUdfQuery<Account>(searchTerm, "Account");
        }
    }
}
