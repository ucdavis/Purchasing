using System.Collections.Generic;
using Dapper;
using Purchasing.Core.Queries;

namespace Purchasing.Web.Services
{
    public interface IAccessQueryService
    {
        IEnumerable<ClosedAccess> GetClosedOrderAccess(string loginId);
        IEnumerable<ClosedAccess> GetClosedOrderAccess(string loginId, int orderId);
        IEnumerable<Access> GetOrderAccess(string loginId);
        IEnumerable<Access> GetOrderAccess(string loginId, int orderId);
        IEnumerable<Access> GetOrderAccessByAdminStatus(string loginId, bool isAdmin);
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
                return conn.Query<ClosedAccess>("select orderid, accessuserid, accesslevel, isadmin from udf_GetClosedOrdersForLogin(@loginId)", new { loginId } );
            }
        }

        public IEnumerable<ClosedAccess> GetClosedOrderAccess(string loginId, int orderId)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<ClosedAccess>("select orderid, accessuserid, accesslevel, isadmin from udf_GetClosedOrdersForLogin(@loginId) where orderid = @orderId", new { loginId, orderId });
            }
        }

        public IEnumerable<Access> GetOrderAccess(string loginId)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<Access>("select orderid, accessuserid, accesslevel, readaccess, editaccess, isadmin  from udf_GetReadAndEditAccessOrdersForLogin(@loginId)", new { loginId });
            }
        }

        public IEnumerable<Access> GetOrderAccess(string loginId, int orderId)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<Access>("select orderid, accessuserid, accesslevel, readaccess, editaccess, isadmin  from udf_GetReadAndEditAccessOrdersForLogin(@loginId) where orderid = @orderId", new { loginId, orderId });
            }
        }

        public IEnumerable<Access> GetOrderAccessByAdminStatus(string loginId, bool isAdmin)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<Access>("select orderid, accessuserid, accesslevel, readaccess, editaccess, isadmin  from udf_GetReadAndEditAccessOrdersForLogin(@loginId) where isadmin = @isAdmin", new { loginId, isAdmin });
            }
        }

        public IEnumerable<EditAccess> GetEditAccess(string loginId)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<EditAccess>("select orderid, accessuserid, accesslevel, isadmin from udf_GetEditAccessOrdersForLogin(@loginId)", new { loginId });
            }
        }
        
        public IEnumerable<ReadAccess> GetReadAccess(string loginId)
        {
            using (var conn = _dbService.GetConnection())
            {
                return conn.Query<ReadAccess>("select orderid, accessuserid, accesslevel, isadmin from udf_GetReadAccessOrdersForLogin(@loginId)", new { loginId });
            }
        }
    }
}