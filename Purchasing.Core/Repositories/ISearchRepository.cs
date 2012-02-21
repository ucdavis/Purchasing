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
        IList<SearchResults.OrderResult> SearchOrders(string searchTerm);
        IList<SearchResults.LineResult> SearchLineItems(string searchTerm);
        IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm);
        IList<SearchResults.CommentResult> SearchComments(string searchTerm);

        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        IList<Commodity> SearchCommodities(string searchTerm);
    }

    public class DevelopmentSearchRepository : ISearchRepository
    {
        public ISession Session { get { return NHibernateSessionManager.Instance.GetSession(); } }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm)
        {
            var query = Session.CreateSQLQuery(
                @"SELECT TOP 100 [Id],[DateCreated],[DeliverTo],[DeliverToEmail],[Justification],[CreatedBy],[RequestNumber] 
                    FROM [PrePurchasing].[dbo].[Orders]
                    WHERE Justification LIKE '%' + :searchTerm + '%'")
                .SetString("searchTerm", searchTerm)
                .SetResultTransformer(Transformers.AliasToBean<SearchResults.OrderResult>());

            return query.List<SearchResults.OrderResult>();
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm)
        {
            var query = Session.CreateSQLQuery(
                @"SELECT TOP 100 o.Id as OrderId, o.RequestNumber,[Quantity],[Unit],[CatalogNumber],[Description],[Url],[Notes],[CommodityId]
                  FROM [PrePurchasing].[dbo].[LineItems] li INNER JOIN
                  [PrePurchasing].[dbo].[Orders] o on o.Id = li.OrderId
                  WHERE Description LIKE '%' + :searchTerm + '%'")
                .SetString("searchTerm", searchTerm)
                .SetResultTransformer(Transformers.AliasToBean<SearchResults.LineResult>());

            return query.List<SearchResults.LineResult>();
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm)
        {
            var query = Session.CreateSQLQuery(
                @"SELECT TOP 100 o.Id as OrderId, o.RequestNumber,c.Name as Question,[Answer]
FROM [PrePurchasing].[dbo].[CustomFieldAnswers] a INNER JOIN
[PrePurchasing].[dbo].[CustomFields] c on c.Id = a.CustomFieldId INNER JOIN
[PrePurchasing].[dbo].[Orders] o on o.Id = a.OrderId
WHERE Answer LIKE '%' + :searchTerm + '%'").SetString("searchTerm", searchTerm)
                .SetResultTransformer(Transformers.AliasToBean<SearchResults.CustomFieldResult>());

            return query.List<SearchResults.CustomFieldResult>();
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm)
        {
            var query = Session.CreateSQLQuery(
                @"SELECT TOP 1000 o.Id as OrderId,o.RequestNumber,[Text],c.[DateCreated],[UserId] as CreatedBy
                  FROM [PrePurchasing].[dbo].[OrderComments] c INNER JOIN
                  [PrePurchasing].[dbo].[Orders] o on o.Id = c.OrderId
                  WHERE Text LIKE '%' + :searchTerm + '%'")
                .SetString("searchTerm", searchTerm)
                .SetResultTransformer(Transformers.AliasToBean<SearchResults.CommentResult>());

            return query.List<SearchResults.CommentResult>();
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

    public class SearchRepository : ISearchRepository
    {
        public ISession Session { get { return NHibernateSessionManager.Instance.GetSession(); } }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm)
        {
            throw new NotImplementedException();
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
