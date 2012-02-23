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
            return PerformQuery<SearchResults.OrderResult>(user, searchTerm, "Order");
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, string user)
        {
            return PerformQuery<SearchResults.LineResult>(user, searchTerm, "Line");
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, string user)
        {
            return PerformQuery<SearchResults.CustomFieldResult>(user, searchTerm, "CustomField");
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, string user)
        {
            return PerformQuery<SearchResults.CommentResult>(user, searchTerm, "Comment");
        }

        private IList<T> PerformQuery<T>(string user, string query, string queryType)
        {
            var q = Session.CreateSQLQuery(
                string.Format("SELECT * FROM [PrePurchasing].[dbo].[udf_Get{0}Results] ('{1}','{2}')", queryType, user,
                              query))
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
