using System.Collections.Generic;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Purchasing.Core.Domain;

namespace Purchasing.Core.Repositories
{
    public interface ICommodityRepository : IRepositoryWithTypedId<Commodity,string>
    {
        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        IList<Commodity> SearchCommodities(string searchTerm);
    }

    public class CommodityRepository : RepositoryWithTypedId<Commodity, string>, ICommodityRepository
    {
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
