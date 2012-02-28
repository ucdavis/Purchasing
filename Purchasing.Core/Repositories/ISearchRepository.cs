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
        ISession Session { get; }
        IList<SearchResults.OrderResult> SearchOrders(string searchTerm, string user);
        IList<SearchResults.LineResult> SearchLineItems(string searchTerm, string user);
        IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, string user);
        IList<SearchResults.CommentResult> SearchComments(string searchTerm, string user);

        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        IList<Commodity> SearchCommodities(string searchTerm);
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

        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        public IList<Commodity> SearchCommodities(string searchTerm)
        {
            //TODO: we can remove groupCode/SubGroupCode if they aren't needed.  Here we just leave them out of the query 
            //TODO: make into FTS once DB indexes are setup
            var query = Session.CreateSQLQuery(
                @"SELECT [Id],[Name]
                ,'' as [GroupCode],'' as [SubGroupCode]
                FROM [vCommodities]
                WHERE Id like '%' + :searchTerm + '%'
                    OR Name like '%' + :searchTerm + '%'")
                .AddEntity(typeof(Commodity))
                .SetString("searchTerm", searchTerm);

            return query.List<Commodity>();
        }
    }
}
