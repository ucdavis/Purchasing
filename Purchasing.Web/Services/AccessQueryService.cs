using System.Collections.Generic;
using Dapper;
using Purchasing.Core.Queries;

namespace Purchasing.Web.Services
{
    public interface IAccessQueryService
    {
        IEnumerable<ClosedAccess> GetClosedOrderAccess(string loginId);
        IEnumerable<ClosedAccess> GetClosedOrderAccess(string loginId, int orderId);
    }

    public class AccessQueryService : IAccessQueryService
    {
        private readonly IDbService _dbService;

        public AccessQueryService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public IEnumerable<ClosedAccess> GetClosedOrderAccess(string loginId)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<ClosedAccess>("select orderid, accessuserid, accesslevel, isadmin from udf_GetClosedOrdersForId(@loginId)", new { loginId } );
            }
        }

        public IEnumerable<ClosedAccess> GetClosedOrderAccess(string loginId, int orderId)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<ClosedAccess>("select orderid, accessuserid, accesslevel, isadmin from udf_GetClosedOrdersForId(@loginId) where orderid = @orderId", new { loginId, orderId });
            }
        }
    }
}