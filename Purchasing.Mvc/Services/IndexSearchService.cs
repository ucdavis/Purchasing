using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Nest;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Mvc.Utility;
using StandardAnalyzer = Lucene.Net.Analysis.Standard.StandardAnalyzer;
using Version = Lucene.Net.Util.Version;

namespace Purchasing.Mvc.Services
{
    public class ElasticSearchService : ISearchService
    {
        private readonly IIndexService _indexService;
        private ElasticClient _client;

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
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds)));

            return results.Hits.Select(h => AutoMapper.Mapper.Map<SearchResults.OrderResult>(h.Source)).ToList();
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds)
        {
            var index = IndexHelper.GetIndexName(Indexes.LineItems);

            var results = _client.Search<SearchResults.LineResult>(
                s =>
                    s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm)))
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds)));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds)
        {
            var index = IndexHelper.GetIndexName(Indexes.CustomAnswers);

            var results = _client.Search<SearchResults.CustomFieldResult>(
                s =>
                    s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm)))
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds)));

            return results.Hits.Select(h => h.Source).ToList();
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds)
        {
            var index = IndexHelper.GetIndexName(Indexes.Comments);

            var results = _client.Search<SearchResults.CommentResult>(
                s =>
                    s.Index(index).Query(q => q.QueryString(qs => qs.Query(searchTerm)))
                        .Filter(f => f.Terms(x => x.OrderId, allowedIds)));

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
                );

            return results.Hits.Select(h => h.Source).ToList();
        }
    }

    /// <summary>
    /// Searches using Lucene indexes
    /// TODO: MUST filter orders by user
    /// </summary>
    public class IndexSearchService : ISearchService
    {
        private readonly IIndexService _indexService;

        public IndexSearchService(IIndexService indexService)
        {
            _indexService = indexService;
        }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.OrderHistory);
            try
            {
                IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.OrderResult.SearchableFields);

                var orderResults = results
                    .Select(scoreDoc => searcher.Doc(scoreDoc.Doc))
                    .Select(doc => new SearchResults.OrderResult
                        {
                            Id = int.Parse(doc.Get("orderid")),
                            Justification = doc.Get("justification"),
                            BusinessPurpose = doc.Get("businesspurpose"),
                            CreatedBy = doc.Get("createdby"),
                            DeliverTo = doc.Get("shipto"),
                            DeliverToEmail = doc.Get("shiptoemail"),
                            RequestNumber = doc.Get("requestnumber"),
                            DateCreated = DateTime.Parse(doc.Get("datecreated")),
                            PoNumber = doc.Get("ponumber"),
                            Tag = doc.Get("tag"),
                            ReferenceNumber = doc.Get("referencenumber"),
                            Approver = doc.Get("approver"),
                            AccountManager = doc.Get("accountmanager"),
                            Purchaser = doc.Get("purchaser")
                        }).ToList();


                return orderResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }

        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.LineItems);
            try
            {
                IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.LineResult.SearchableFields);

                var lineResults = results
                    .Select(scoreDoc => searcher.Doc(scoreDoc.Doc))
                    .Select(doc => new SearchResults.LineResult
                    {
                        OrderId = int.Parse(doc.Get("orderid")),
                        Description = doc.Get("description"),
                        Url = doc.Get("url"),
                        Notes = doc.Get("notes"),
                        CatalogNumber = doc.Get("catalognumber"),
                        CommodityId = doc.Get("commodityid"),
                        ReceivedNotes = doc.Get("receivednotes"),
                        PaidNotes = doc.Get("paidnotes"),
                        RequestNumber = doc.Get("requestnumber"),
                        Quantity = decimal.Parse(doc.Get("quantity")),
                        Unit = doc.Get("unit")
                    }).ToList();

                return lineResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }

        }

        public IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.CustomAnswers);
            try
            {
                IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.CustomFieldResult.SearchableFields);

                var customFieldResults = results
                    .Select(scoreDoc => searcher.Doc(scoreDoc.Doc))
                    .Select(doc => new SearchResults.CustomFieldResult
                    {
                        OrderId = int.Parse(doc.Get("orderid")),
                        RequestNumber = doc.Get("requestnumber"),
                        Answer = doc.Get("answer"),
                        Question = doc.Get("question")
                    }).ToList();


                return customFieldResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }
        }

        public IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds)
        {
            var searcher = _indexService.GetIndexSearcherFor(Indexes.Comments);
            try
            {
                IEnumerable<ScoreDoc> results = SearchIndex(searcher, allowedIds, searchTerm, SearchResults.CommentResult.SearchableFields);

                var commentResults = results
                    .Select(scoreDoc => searcher.Doc(scoreDoc.Doc))
                    .Select(doc => new SearchResults.CommentResult()
                    {
                        OrderId = int.Parse(doc.Get("orderid")),
                        RequestNumber = doc.Get("requestnumber"),
                        Text = doc.Get("text"),
                        CreatedBy = doc.Get("createdby"),
                        DateCreated = DateTime.Parse(doc.Get("datecreated"))
                    }).ToList();

                return commentResults;
            }
            finally //The exception will be caught downstream
            {
                searcher.Close();
                searcher.Dispose();
            }

        }

        public IList<IdAndName> SearchCommodities(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Commodities, searchTerm);
        }

        public IList<IdAndName> SearchVendors(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Vendors, searchTerm);
        }

        public IList<IdAndName> SearchAccounts(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Accounts, searchTerm);
        }

        public IList<IdAndName> SearchBuildings(string searchTerm)
        {
            return SearchLookupIndex(Indexes.Buildings, searchTerm);
        }

        private IList<IdAndName> SearchLookupIndex(Indexes index, string searchTerm, int topN = 20)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<IdAndName>();
            }
            var searcher = _indexService.GetIndexSearcherFor(index);
            
            //Default to standard analyzer-- id field is tokenized into searchid non-stored field
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            try
            {
                var termsQuery = new MultiFieldQueryParser(Version.LUCENE_30, new[] { "searchid", "name" }, analyzer).Parse(searchTerm);
                var results = searcher.Search(termsQuery, topN).ScoreDocs;

                var entities = results
                    .Select(scoreDoc => searcher.Doc(scoreDoc.Doc))
                    .Select(doc => new IdAndName(doc.Get("id"), doc.Get("name"))).ToList();

                return entities;
            }
            catch (ParseException)
            {
                return new List<IdAndName>();
            }
            finally
            {
                    analyzer.Close();
                    searcher.Close();
                    searcher.Dispose();
            }
       


            
        }

        public IList<OrderHistory> GetOrdersByWorkgroups(IEnumerable<Workgroup> workgroups, DateTime createdAfter, DateTime createdBefore)
        {
            var workgroupIds = workgroups.Select(x => x.Id).Distinct().ToArray();

            var searcher = _indexService.GetIndexSearcherFor(Indexes.OrderHistory);

            //Search for all orders within the given workgroups, created in the range desired
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            Query query = new QueryParser(Version.LUCENE_30, "workgroupid", analyzer).Parse(string.Join(" ", workgroupIds));
            var filter = NumericRangeFilter.NewLongRange("datecreatedticks", createdAfter.Ticks, createdBefore.AddDays(1).Ticks, true, true);

            //Need to return all matching orders
            var docs = searcher.Search(query, filter, int.MaxValue).ScoreDocs;
            var orderHistory = new List<OrderHistory>();

            foreach (var scoredoc in docs)
            {
                var doc = searcher.Doc(scoredoc.Doc);

                var history = new OrderHistory();

                foreach (var prop in history.GetType().GetProperties())
                {
                    if (!string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        prop.SetValue(history, Convert.ChangeType(doc.Get(prop.Name.ToLower()), prop.PropertyType), null);
                    }
                }

                orderHistory.Add(history);
            }

            analyzer.Close();
            searcher.Close();
            searcher.Dispose();

            return orderHistory;
        }

        /// <summary>
        /// Searches an index by the searchTerm and across searchableFields, filtering the results to only within the given orderIds
        /// </summary>
        /// <returns>ScoreDoc hits</returns>
        private IEnumerable<ScoreDoc> SearchIndex(IndexSearcher searcher, IEnumerable<int> filteredOrderIds, string searchTerm, string[] searchableFields, int topN = 20)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            try
            {   
                var orderIds = filteredOrderIds as int[] ?? filteredOrderIds.ToArray();

                if (BooleanQuery.MaxClauseCount < orderIds.Length)
                {
                    BooleanQuery.MaxClauseCount = orderIds.Length + 1;
                }

                Query accessQuery = new QueryParser(Version.LUCENE_30, "orderid", analyzer).Parse(string.Join(" ", orderIds));
                var termsQuery =
                    new MultiFieldQueryParser(Version.LUCENE_30, searchableFields, analyzer).Parse(searchTerm);
                var results = searcher.Search(termsQuery, new CachingWrapperFilter(new QueryWrapperFilter(accessQuery)), topN).ScoreDocs;                

                return results;
            }
            catch (ParseException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }                
            finally
            {
                analyzer.Close();
            }

        }
    }
}