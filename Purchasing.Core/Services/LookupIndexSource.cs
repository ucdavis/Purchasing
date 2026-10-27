using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Purchasing.Core.Domain;

namespace Purchasing.Core.Services
{
    // Keep the local runner and scheduled rebuilds on the same lookup projections.
    internal static class LookupIndexSource
    {
        internal const string AccountsSql = "SELECT [Id], [Name] FROM vAccounts WHERE [IsActive] = 1";
        internal const string BuildingsSql = "SELECT [Id], [BuildingName] FROM vBuildings";
        internal const string CommoditiesSql = "SELECT [Id], [Name] FROM vCommodities WHERE [IsActive] = 1";
        internal const string VendorsSql = "SELECT [Id], [Name] FROM vVendors WHERE [IsActive] = 1";

        internal static SearchProjection Read(IDbService db, Indexes index, string after, int size, string[] ids = null)
        {
            using var connection = db.GetConnection();
            // Every lookup has a unique Id. Keep its source collation so the keyset can use that index.
            var suffix = ids == null
                ? " WHERE (@after IS NULL OR [Id] > @after) ORDER BY [Id]"
                : " WHERE [Id] IN @ids";
            SearchProjection Read<T>(string sql) where T : class => new SearchProjection<T>(index,
                connection.Query<T>($"SELECT TOP (@size) * FROM ({sql}) projected{suffix}",
                    new { after, size, ids }, commandTimeout: 300).ToList());
            return index switch
            {
                Indexes.Accounts => Read<Account>(AccountsSql),
                Indexes.Buildings => Read<Building>(BuildingsSql),
                Indexes.Commodities => Read<Commodity>(CommoditiesSql),
                Indexes.Vendors => Read<Vendor>(VendorsSql),
                _ => throw new ArgumentException("Not a lookup index.")
            };
        }
    }
}
