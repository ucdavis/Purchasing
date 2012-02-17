using System;
using System.Collections.Generic;
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
        IList<Order> SearchOrders(string searchTerm);
        IList<LineItem> SearchLineItems(string searchTerm);
        IList<CustomFieldAnswer> SearchCustomFieldAnswers(string searchTerm);
        IList<OrderComment> SearchComments(string searchTerm);

        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        IList<Commodity> SearchCommodities(string searchTerm);
    }

    public class SearchRepository : ISearchRepository
    {
        public ISession Session { get { return NHibernateSessionManager.Instance.GetSession(); } }

        public IList<Order> SearchOrders(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<LineItem> SearchLineItems(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<CustomFieldAnswer> SearchCustomFieldAnswers(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<OrderComment> SearchComments(string searchTerm)
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
