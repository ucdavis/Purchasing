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
            if (allowedIds.Length == 0) // no results if you aren't allowed to see anything
            {
                return new List<SearchResults.OrderResult>();
            }

            var index = IndexHelper.GetIndexName(Indexes.OrderHistory);

            var results = _client.Search<OrderHistory>(
                s =>
                    s.Index(index)
                        .Query(
                            q => q.QueryString(qs => qs.Query(searchTerm))
                        )
                        .PostFilter(f => f.ConstantScore(c => c.Filter(x => x.Terms(t => t.Field(q => q.OrderId).Terms(allowedIds)))))
                        .Sort(sort => sort.Descending(d => d.LastActionDate))
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => AutoMapper.Mapper.Map<SearchResults.OrderResult>(h.Source)).ToList();
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds)
        {
            if (allowedIds.Length == 0) // no results if you aren't allowed to see anything
            {
                return new List<SearchResults.LineResult>();
            }

            var index = IndexHelper.GetIndexName(Indexes.LineItems);

            var results = _client.Search<SearchResults.LineResult>(
                s =>
                    s.Index(index)
                        .Query(
                            q => q.QueryString(qs => qs.Query(searchTerm))
                        )
                        .PostFilter(f => f.ConstantScore(c => c.Filter(x => x.Terms(t => t.Field(q => q.OrderId).Terms(allowedIds)))))
                        .Sort(sort => sort.Descending(d => d.OrderId))
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds)
        {
            if (allowedIds.Length == 0) // no results if you aren't allowed to see anything
            {
                return new List<SearchResults.CustomFieldResult>();
            }

            var index = IndexHelper.GetIndexName(Indexes.CustomAnswers);

            var results = _client.Search<SearchResults.CustomFieldResult>(
                s =>
                    s.Index(index)
                        .Query(
                            q => q.QueryString(qs => qs.Query(searchTerm))
                        )
                        .PostFilter(f => f.ConstantScore(c => c.Filter(x => x.Terms(t => t.Field(q => q.OrderId).Terms(allowedIds)))))
                        .Sort(sort => sort.Descending(d => d.OrderId))
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds)
        {
            if (allowedIds.Length == 0) // no results if you aren't allowed to see anything
            {
                return new List<SearchResults.CommentResult>();
            }

            var index = IndexHelper.GetIndexName(Indexes.Comments);

            var results = _client.Search<SearchResults.CommentResult>(
                s =>
                    s.Index(index)
                        .Query(
                            q => q.QueryString(qs => qs.Query(searchTerm))
                        )
                        .PostFilter(f=>f.ConstantScore(c=>c.Filter(x=>x.Terms(t=>t.Field(q=>q.OrderId).Terms(allowedIds)))))
                        .Sort(sort => sort.Descending(d => d.DateCreated))
                        .Size(MaxSeachResults));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<SearchResults.CommentResult> GetLatestComments(int count, int[] allowedIds)
        {
            if (allowedIds.Length == 0) // no results if you aren't allowed to see anything
            {
                return new List<SearchResults.CommentResult>();
            }

            var index = IndexHelper.GetIndexName(Indexes.Comments);

            var results = _client.Search<SearchResults.CommentResult>(
                s =>
                    s.Index(index)
                        .PostFilter(f => f.ConstantScore(c => c.Filter(x => x.Terms(t => t.Field(q => q.OrderId).Terms(allowedIds)))))
                        .Sort(sort => sort.Descending(d => d.DateCreated))
                        .Size(count));

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

        public IList<OrderHistory> GetOrdersByWorkgroups(IEnumerable<Workgroup> workgroups, DateTime createdAfter, DateTime createdBefore, int size = 1000)
        {
            var index = IndexHelper.GetIndexName(Indexes.OrderHistory);
            var workgroupIds = workgroups.Select(w => w.Id).ToArray();
            var results = _client.Search<OrderHistory>(
                s =>
                    s.Index(index)
                        .Size(size)
                        .Query(f => f.DateRange(r => r.Field(o => o.DateCreated).GreaterThan(createdAfter).LessThan(createdBefore))
                        && f.Terms(o => o.Field(field => field.WorkgroupId).Terms(workgroupIds)))
                        
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
                    .Query(f => f.DateRange(r => r.Field(o => o.OrderCreated).GreaterThan(createdAfter).LessThan(createBefore))
                                    && f.Term(x => x.IsComplete, true)
                                    && f.Terms(o => o.Field(field => field.WorkgroupId).Terms(workgroupIds)))
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
                AverageTimeToAccountManagers = results.Aggregations.Average("AverageTimeToAccountManagers").Value,
                AverageTimeToApprover = results.Aggregations.Average("AverageTimeToRoleComplete").Value,
                AverageTimeToCompletion = results.Aggregations.Average("AverageTimeToCompletion").Value,
                AverageTimeToPurchaser = results.Aggregations.Average("AverageTimeToPurchaser").Value
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

            // get the .keyword version of the index, for aggregates (since we aren't full text searching against the field)
            roleNames = roleNames + ".keyword";

            var results = _client.Search<OrderTrackingEntity>(
                s => s.Index(index)
                    .Size(size)
                    .Query(f => f.DateRange(r => r.Field(o => o.OrderCreated).GreaterThan(createdAfter).LessThan(createBefore))
                                 && f.Term(x => x.Status, "complete")
                                 && f.Terms(o => o.Field(field => field.WorkgroupId).Terms(workgroupIds)))
                    .Aggregations(
                        a =>
                            a.Terms("RolePercentiles", t => t.Field(roleNames)
                                    .Aggregations(b=> b.Percentiles("PercentileTimes", p=> p.Field(timeField)
                                    .Percents(new double[] {0,25,50,75,100})))
                                )
                             .Average("AverageTimeToRoleComplete", avg => avg.Field(timeField)))
                );
            var names = results.Aggregations.Terms("RolePercentiles").Buckets.Select(n => n.Key + "; n=" + n.DocCount.ToString()).ToArray();
            var percentilesInRole =
                results.Aggregations.Terms("RolePercentiles")
                    .Buckets.Select(user => user.PercentilesBucket("PercentileTimes"))
                    .Select(
                        uservalue => uservalue.Items.OrderBy(o => o.Percentile).Select(a => (a.Value.HasValue ? a.Value.Value : 0) / 1440).ToArray() // converted to days to help with scale
                            )
                    .ToList();


            return new OrderTrackingAggregationByRole
            {
                OrderTrackingEntities = results.Hits.Select(h => h.Source).ToList(),
                AverageTimeToRoleComplete = results.Aggregations.Average("AverageTimeToRoleComplete").Value / 1440,
                NamesInRole = names,
                PercentilesForRole = percentilesInRole
            };
        }
    }
}