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
                        .Query(q => q.Terms(o => o.WorkgroupId, workgroupIds))
                        .Filter(f => f.Range(r => r.OnField(o => o.DateCreated).Greater(createdAfter).Lower(createdBefore)))
                        .Size(int.MaxValue)
                );

            return results.Hits.Select(h => h.Source).ToList();
        }
    }
}