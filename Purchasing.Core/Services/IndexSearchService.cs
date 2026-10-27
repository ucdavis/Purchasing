using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;

namespace Purchasing.Core.Services
{
    public class ElasticSearchService : ISearchService
    {
        private readonly IndexBackend _backend;
        public ElasticSearchService(SearchBackendSet backends) { _backend = backends.Reader; }

        private static JObject Permissions(int[] ids) => JObject.FromObject(new
        {
            constant_score = new { filter = new { terms = new { orderId = ids } } }
        });

        private List<T> Search<T>(Indexes index, string term, int[] allowedIds = null, string sort = null, int size = 1000)
        {
            if (allowedIds != null && allowedIds.Length == 0) return new List<T>();
            var query = new JObject { ["size"] = size };
            if (term != null) query["query"] = JObject.FromObject(new { query_string = new { query = term } });
            if (allowedIds != null) query["post_filter"] = Permissions(allowedIds);
            if (sort != null) query["sort"] = new JArray(new JObject { [sort] = "desc" });
            return IndexBackend.Hits<T>(_backend.Search(index, query));
        }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, int[] allowedIds) =>
            Search<OrderHistory>(Indexes.OrderHistory, searchTerm, allowedIds, "lastActionDate").Select(SearchResults.OrderResult.FromHistory).ToList();
        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds) =>
            Search<SearchResults.LineResult>(Indexes.LineItems, searchTerm, allowedIds, "orderId");
        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds) =>
            Search<SearchResults.CustomFieldResult>(Indexes.CustomAnswers, searchTerm, allowedIds, "orderId");
        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds) =>
            Search<SearchResults.CommentResult>(Indexes.Comments, searchTerm, allowedIds, "dateCreated");
        public IList<SearchResults.CommentResult> GetLatestComments(int count, int[] allowedIds) =>
            Search<SearchResults.CommentResult>(Indexes.Comments, null, allowedIds, "dateCreated", count);
        public IList<IdAndName> SearchCommodities(string searchTerm) =>
            Search<Commodity>(Indexes.Commodities, searchTerm, size: 10).Select(x => new IdAndName(x.Id, x.Name)).ToList();
        public IList<IdAndName> SearchVendors(string searchTerm) =>
            Search<Vendor>(Indexes.Vendors, searchTerm, size: 10).Select(x => new IdAndName(x.Id, x.Name)).ToList();
        public IList<IdAndName> SearchAccounts(string searchTerm) =>
            Search<Account>(Indexes.Accounts, searchTerm, size: 10).Select(x => new IdAndName(x.Id, x.Name)).ToList();
        public IList<IdAndName> SearchBuildings(string searchTerm) =>
            Search<Building>(Indexes.Buildings, searchTerm, size: 10).Select(x => new IdAndName(x.Id, x.BuildingName)).ToList();

        public IndexedList<OrderHistory> GetOrderHistory(int[] orderIds, DateTime? startDate, DateTime? endDate,
            DateTime? startLastActionDate, DateTime? endLastActionDate, string statusId)
        {
            if (orderIds.Length == 0) return HistoryResults(new List<OrderHistory>());
            var filters = new JArray();
            if (startLastActionDate.HasValue)
                filters.Add(Range("lastActionDate", "gte", startLastActionDate.Value, "lte", endLastActionDate ?? DateTime.UtcNow.AddDays(1)));
            if (startDate.HasValue) filters.Add(Range("dateCreated", "gte", startDate.Value));
            if (endDate.HasValue) filters.Add(Range("dateCreated", "lte", endDate.Value));
            if (!string.IsNullOrWhiteSpace(statusId))
                filters.Add(JObject.FromObject(new { term = new { statusId = new { value = statusId.ToLowerInvariant() } } }));
            var query = new JObject
            {
                ["query"] = new JObject { ["bool"] = new JObject { ["must"] = filters } },
                ["post_filter"] = Permissions(orderIds),
                ["sort"] = new JArray(new JObject { ["lastActionDate"] = "desc" }),
                ["size"] = Math.Min(orderIds.Length, 10000)
            };
            return HistoryResults(IndexBackend.Hits<OrderHistory>(_backend.Search(Indexes.OrderHistory, query)));
        }

        private static IndexedList<OrderHistory> HistoryResults(List<OrderHistory> results) => new IndexedList<OrderHistory>
        {
            Results = results,
            // Retain the existing display estimate until timestamp accuracy is addressed in #677.
            LastModified = DateTime.UtcNow.ToPacificTime().AddMinutes(-5).ToLocalTime()
        };

        private static JObject Range(string field, string comparison, DateTime value, string otherComparison = null, DateTime? otherValue = null)
        {
            var bounds = new JObject { [comparison] = value.ToString("O", CultureInfo.InvariantCulture) };
            if (otherComparison != null) bounds[otherComparison] = otherValue.Value.ToString("O", CultureInfo.InvariantCulture);
            return new JObject { ["range"] = new JObject { [field] = bounds } };
        }

        private static JObject WorkgroupQuery(int[] workgroups, string dateField, DateTime after, DateTime before, int size, JObject extra = null)
        {
            var filters = new JArray(Range(dateField, "gt", after, "lt", before),
                JObject.FromObject(new { terms = new { workgroupId = workgroups } }));
            if (extra != null) filters.Add(extra);
            return new JObject { ["size"] = size, ["query"] = new JObject { ["bool"] = new JObject { ["filter"] = filters } } };
        }

        public IList<OrderHistory> GetOrdersByWorkgroups(IEnumerable<Workgroup> workgroups, DateTime createdAfter, DateTime createdBefore, int size = 1000)
        {
            var ids = workgroups.Select(w => w.Id).ToArray();
            if (ids.Length == 0) return new List<OrderHistory>();
            return IndexBackend.Hits<OrderHistory>(_backend.Search(Indexes.OrderHistory, WorkgroupQuery(ids, "dateCreated", createdAfter, createdBefore, size)));
        }

        private static JObject Average(string field) => new JObject { ["avg"] = new JObject { ["field"] = field } };
        private static double? AverageValue(JObject result, string name) => result["aggregations"]?[name]?["value"]?.Value<double?>();

        public OrderTrackingAggregation GetOrderTrackingEntities(IEnumerable<Workgroup> workgroups, DateTime createdAfter, DateTime createBefore, int size = 1000)
        {
            var ids = workgroups.Select(w => w.Id).ToArray();
            if (ids.Length == 0) return new OrderTrackingAggregation { OrderTrackingEntities = new List<OrderTrackingEntity>() };
            var query = WorkgroupQuery(ids, "orderCreated", createdAfter, createBefore, size, JObject.FromObject(new { term = new { isComplete = true } }));
            query["aggs"] = new JObject
            {
                ["AverageTimeToCompletion"] = Average("minutesToCompletion"),
                ["AverageTimeToRoleComplete"] = Average("minutesToApprove"),
                ["AverageTimeToAccountManagers"] = Average("minutesToAccountManagerComplete"),
                ["AverageTimeToPurchaser"] = Average("minutesToPurchaserComplete")
            };
            var result = _backend.Search(Indexes.OrderTracking, query);
            return new OrderTrackingAggregation
            {
                OrderTrackingEntities = IndexBackend.Hits<OrderTrackingEntity>(result),
                AverageTimeToCompletion = AverageValue(result, "AverageTimeToCompletion"),
                AverageTimeToApprover = AverageValue(result, "AverageTimeToRoleComplete"),
                AverageTimeToAccountManagers = AverageValue(result, "AverageTimeToAccountManagers"),
                AverageTimeToPurchaser = AverageValue(result, "AverageTimeToPurchaser")
            };
        }

        public OrderTrackingAggregationByRole GetOrderTrackingEntitiesByRole(IEnumerable<Workgroup> workgroups,
            DateTime createdAfter, DateTime createBefore, string role, int size)
        {
            var ids = workgroups.Select(w => w.Id).ToArray();
            if (ids.Length == 0) return new OrderTrackingAggregationByRole
            {
                OrderTrackingEntities = new List<OrderTrackingEntity>(), NamesInRole = Array.Empty<string>(), PercentilesForRole = new List<double[]>()
            };
            var roleField = role == "Approver" ? "approverId" : role == "AccountManager" ? "accountManagerId" : "purchaserId";
            var timeField = role == "Approver" ? "minutesToApprove" : role == "AccountManager" ? "minutesToAccountManagerComplete" : "minutesToPurchaserComplete";
            var query = WorkgroupQuery(ids, "orderCreated", createdAfter, createBefore, size, JObject.FromObject(new { term = new { status = "complete" } }));
            query["aggs"] = new JObject
            {
                ["RolePercentiles"] = JObject.FromObject(new
                {
                    terms = new { field = roleField + ".keyword" },
                    aggs = new { PercentileTimes = new { percentiles = new { field = timeField, percents = new[] { 0, 25, 50, 75, 100 } } } }
                }),
                ["AverageTimeToRoleComplete"] = Average(timeField)
            };
            var result = _backend.Search(Indexes.OrderTracking, query);
            var buckets = result["aggregations"]?["RolePercentiles"]?["buckets"] as JArray
                ?? throw new SearchBackendException($"{_backend.Name}: tracking report returned no aggregation buckets.");
            return new OrderTrackingAggregationByRole
            {
                OrderTrackingEntities = IndexBackend.Hits<OrderTrackingEntity>(result),
                AverageTimeToRoleComplete = AverageValue(result, "AverageTimeToRoleComplete") / 1440,
                NamesInRole = buckets.Select(b => b.Value<string>("key") + "; n=" + b.Value<long>("doc_count")).ToArray(),
                PercentilesForRole = buckets.Select(b => ((JObject)b["PercentileTimes"]["values"]).Properties()
                    .OrderBy(p => double.Parse(p.Name, CultureInfo.InvariantCulture))
                    .Select(p => (p.Value.Value<double?>() ?? 0) / 1440).ToArray()).ToList()
            };
        }
    }
}
