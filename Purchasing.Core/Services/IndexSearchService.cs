using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;


namespace Purchasing.Core.Services
{
    public class ElasticSearchService : ISearchService
    {
        private readonly IIndexService _indexService;
        private ElasticClient _client;
        private const int MaxSeachResults = 1000;

        public ElasticSearchService(IIndexService indexService)
        {
            _indexService = indexService;
            _client = indexService.GetIndexClient();
        }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, int[] allowedIds)
        {
            var index = IndexHelper.GetIndexName(Indexes.OrderHistory);

            var results = _client.Search<OrderHistory>(
                s =>
                    s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm)))
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds))
                        .SortDescending(x => x.LastActionDate)
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => AutoMapper.Mapper.Map<SearchResults.OrderResult>(h.Source)).ToList();
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds)
        {
            var index = IndexHelper.GetIndexName(Indexes.LineItems);

            var results = _client.Search<SearchResults.LineResult>(
                s =>
                    s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm)))
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds))
                        .SortDescending(x=>x.OrderId)
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds)
        {
            var index = IndexHelper.GetIndexName(Indexes.CustomAnswers);

            var results = _client.Search<SearchResults.CustomFieldResult>(
                s =>
                    s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm)))
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds))
                        .SortDescending(x=>x.OrderId)
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds)
        {
            var index = IndexHelper.GetIndexName(Indexes.Comments);

            var results = _client.Search<SearchResults.CommentResult>(
                s =>
                    s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm)))
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds))
                        .SortDescending(x => x.DateCreated)
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<IdAndName> SearchCommodities(string searchTerm)
        {
            var index = IndexHelper.GetIndexName(Indexes.Commodities);

            var results = _client.Search<Commodity>(
                s => s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm))));

            return results.Hits.Select(h => new IdAndName(h.Source.Id, h.Source.Name)).ToList();
        }

        public IList<IdAndName> SearchVendors(string searchTerm)
        {
            var index = IndexHelper.GetIndexName(Indexes.Vendors);

            var results = _client.Search<Vendor>(
                s => s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm))));

            return results.Hits.Select(h => new IdAndName(h.Source.Id, h.Source.Name)).ToList();
        }

        public IList<IdAndName> SearchAccounts(string searchTerm)
        {
            var index = IndexHelper.GetIndexName(Indexes.Accounts);

            var results = _client.Search<Account>(
                s => s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm))));

            return results.Hits.Select(h => new IdAndName(h.Source.Id, h.Source.Name)).ToList();
        }

        public IList<IdAndName> SearchBuildings(string searchTerm)
        {
            var index = IndexHelper.GetIndexName(Indexes.Buildings);

            var results = _client.Search<Building>(
                s => s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm))));

            return results.Hits.Select(h => new IdAndName(h.Source.Id, h.Source.BuildingName)).ToList();
        }

        public IList<OrderHistory> GetOrdersByWorkgroups(IEnumerable<Workgroup> workgroups, DateTime createdAfter, DateTime createdBefore)
        {
            var index = IndexHelper.GetIndexName(Indexes.OrderHistory);
            var workgroupIds = workgroups.Select(w => w.Id).ToArray();
            var results = _client.Search<OrderHistory>(
                s =>
                    s.Index(index)
                        .Filter(f => f.Range(r => r.OnField(o => o.DateCreated).Greater(createdAfter).Lower(createdBefore))
                        && f.Terms(o => o.WorkgroupId, workgroupIds))
                        .Size(int.MaxValue)
                );

            return results.Hits.Select(h => h.Source).ToList();
        }

        public OrderTrackingAggregation GetOrderTrackingEntities(IEnumerable<Workgroup> workgroups,
            DateTime createdAfter, DateTime createBefore, int size = 1000)
        {
            var index = IndexHelper.GetIndexName(Indexes.OrderTracking);
            var workgroupIds = workgroups.Select(w => w.Id).ToArray();
            
            var results = _client.Search<OrderTrackingEntity>(
                s => s.Index(index)
                    .Size(size)
                    .Query(a=> a.Filtered(fq=> fq.Query(q=> q.MatchAll()).Filter(f => f.Range(r => r.OnField(o => o.OrderCreated).Greater(createdAfter).Lower(createBefore))
                                 && f.Term(x => x.IsComplete, true)
                                 && f.Terms(o => o.WorkgroupId, workgroupIds))))
                    .Aggregations(
                        a =>
                            a.Average("AverageTimeToCompletion", avg => avg.Field(x => x.MinutesToCompletion))
                                .Average("AverageTimeToRoleComplete", avg => avg.Field(x => x.MinutesToApprove))
                                .Average("AverageTimeToAccountManagers",
                                    avg => avg.Field(x => x.MinutesToAccountManagerComplete))
                                .Average("AverageTimeToPurchaser", avg => avg.Field(x => x.MinutesToPurchaserComplete)))
                );
            

            return new OrderTrackingAggregation
            {
                OrderTrackingEntities = results.Hits.Select(h => h.Source).ToList(),
                AverageTimeToAccountManagers = results.Aggs.Average("AverageTimeToAccountManagers").Value,
                AverageTimeToApprover = results.Aggs.Average("AverageTimeToRoleComplete").Value,
                AverageTimeToCompletion = results.Aggs.Average("AverageTimeToCompletion").Value,
                AverageTimeToPurchaser = results.Aggs.Average("AverageTimeToPurchaser").Value
            };
        }

        public OrderTrackingAggregationByRole GetOrderTrackingEntitiesByRole(IEnumerable<Workgroup> workgroups,
            DateTime createdAfter, DateTime createBefore, string role, int size)
        {
            var index = IndexHelper.GetIndexName(Indexes.OrderTracking);
            var workgroupIds = workgroups.Select(w => w.Id).ToArray();
            
            string roleNames = "purchaserId";
            string timeField = "minutesToPurchaserComplete";

            if (role == "Approver")
            {
                roleNames = "approverId";
                timeField = "minutesToApprove";
            }
            if (role == "AccountManager")
            {
                roleNames = "accountManagerId";
                timeField = "minutesToAccountManagerComplete";
            }

            var results = _client.Search<OrderTrackingEntity>(
                s => s.Index(index)
                    .Size(size)
                    .Query(a => a.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(r => r.OnField(o => o.OrderCreated).Greater(createdAfter).Lower(createBefore))
                                 && f.Term(x => x.Status, "complete")
                                 && f.Terms(o => o.WorkgroupId, workgroupIds))))
                    .Aggregations(
                        a =>
                            a.Terms("RolePercentiles", t => t.Field(roleNames)
                                    .Aggregations(b=> b.Percentiles("PercentileTimes", p=> p.Field(timeField)
                                    .Percentages(new double[] {0,25,50,75,100})))
                                )
                             .Average("AverageTimeToRoleComplete", avg => avg.Field(timeField)))
                );
            var names = results.Aggs.Terms("RolePercentiles").Items.Select(n => n.Key + "; n=" + n.DocCount.ToString()).ToArray();
            var percentilesInRole =
                results.Aggs.Terms("RolePercentiles")
                    .Items.Select(user => (PercentilesMetric) user.Aggregations["PercentileTimes"])
                    .Select(
                        uservalue => uservalue.Items.OrderBy(o => o.Percentile).Select(a => a.Value / 1440).ToArray() // converted to days to help with scale
                            )
                    .ToList();


            return new OrderTrackingAggregationByRole
            {
                OrderTrackingEntities = results.Hits.Select(h => h.Source).ToList(),
                AverageTimeToRoleComplete = results.Aggs.Average("AverageTimeToRoleComplete").Value,
                NamesInRole = names,
                PercentilesForRole = percentilesInRole
            };
        }
    }
}