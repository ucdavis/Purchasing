using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoMapper.Internal;
using Dapper;
using Nest;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;

namespace Purchasing.Core.Services
{
    public interface IIndexService
    {
        void SetIndexRoot(string root);

        void CreateAccountsIndex();
        void CreateBuildingsIndex();
        void CreateCommentsIndex();
        void CreateCommoditiesIndex();
        void CreateCustomAnswersIndex();
        void CreateHistoricalOrderIndex();
        void CreateLineItemsIndex();
        void CreateVendorsIndex();

        void UpdateOrderIndexes();

        DateTime LastModified(Indexes index);
        int NumRecords(Indexes index);
        void UpdateCommentsIndex();

        ElasticClient GetIndexClient();
        void CreateTrackingIndex();

        /// <summary>
        /// return just the orders within the given date ranges
        /// </summary>
        /// <param name="orderids"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="startLastActionDate"></param>
        /// <param name="endLastActionDate"></param>
        /// <returns></returns>
        IndexedList<OrderHistory> GetOrderHistory(int[] orderids, DateTime? startDate, DateTime? endDate,
            DateTime? startLastActionDate, DateTime? endLastActionDate, string statusId);
    }
    public class Person
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public virtual string Middle { get; set; }
    }
    public class ElasticSearchIndexService : IIndexService
    {
        private readonly IDbService _dbService;
        private ElasticClient _client;
        private const int MaxReturnValues = 10000; //This was 15,000 but ElasticSearch Errors out if it is that big. Tested with 12,000


        public ElasticSearchIndexService(IDbService dbService)
        {
            _dbService = dbService;
            
            var settings = new ConnectionSettings(new Uri(ConfigurationManager.AppSettings["ElasticSearchUrl"]));
            settings.DefaultIndex("prepurchasing");

            _client = new ElasticClient(settings);
        }

        public void SetIndexRoot(string root)
        {
            throw new NotImplementedException();
        }

        public void CreateAccountsIndex()
        {
            WriteIndex<Account>("SELECT [Id], [Name] FROM vAccounts WHERE [IsActive] = 1", Indexes.Accounts);
        }

        public void CreateBuildingsIndex()
        {
            WriteIndex<Building>("SELECT [Id], [BuildingName] FROM vBuildings", Indexes.Buildings);
        }

        public void CreateCommentsIndex()
        {
            WriteIndex<SearchResults.CommentResult>(
                "SELECT [Id], [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults",
                Indexes.Comments);
        }

        public void CreateCommoditiesIndex()
        {
            WriteIndex<Commodity>("SELECT [Id], [Name] FROM vCommodities WHERE [IsActive] = 1", Indexes.Commodities);
        }

        public void CreateCustomAnswersIndex()
        {
            WriteIndex<SearchResults.CustomFieldResult>(
                "SELECT [Id], [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults",
                Indexes.CustomAnswers);
        }

        public void CreateHistoricalOrderIndex()
        {
            IEnumerable<OrderHistory> orderHistoryEntries;

            using (var conn = _dbService.GetConnection())
            {
                orderHistoryEntries = conn.Query<OrderHistory>("SELECT * FROM vOrderHistory");
            }

            WriteIndex(orderHistoryEntries, Indexes.OrderHistory, o => o.OrderId);
        }

        public void CreateLineItemsIndex()
        {
            WriteIndex<SearchResults.LineResult>(
                "SELECT [Id], [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes], [PaidNotes] FROM vLineResults",
                Indexes.LineItems
            );
        }

        public void CreateVendorsIndex()
        {
            WriteIndex<Vendor>("SELECT [Id], [Name] FROM vVendors WHERE [IsActive] = 1", Indexes.Vendors);
        }

        public void CreateTrackingIndex()
        {
            using (var conn = _dbService.GetConnection())
            {
                var orderTrackings =
                    conn.Query<OrderTrackingDto>(@"select * from [vOrderTrackingIndex] ORDER BY ActionDate DESC", Indexes.OrderTracking).ToList();

                var entities = ProcessTrackingEntities(orderTrackings);

                WriteIndex(entities, Indexes.OrderTracking);
            }
        }

        public void UpdateOrderIndexes()
        {
            var lastUpdate = DateTime.UtcNow.ToPacificTime().AddMinutes(-10); //10 minutes ago.
            //var lastUpdate = DateTime.UtcNow.ToPacificTime().Subtract(TimeSpan.FromMinutes(10)); //10 minutes ago.

            IEnumerable<OrderHistory> orderHistoryEntries = null;
            IEnumerable<SearchResults.LineResult> lineItems = null;
            IEnumerable<SearchResults.CustomFieldResult> customAnswers = null;
            IEnumerable<OrderTrackingEntity> orderTrackingEntities = null;

            int[] updatedOrderIds;

            using (var conn = _dbService.GetConnection())
            {
                updatedOrderIds = conn.Query<int>("select DISTINCT OrderId from OrderTracking where DateCreated > @lastUpdate", new { lastUpdate }).ToArray();

                if (updatedOrderIds.Any())
                {
                    var updatedOrderIdsParameter = new StringBuilder();
                    foreach (var updatedOrderId in updatedOrderIds)
                    {
                        updatedOrderIdsParameter.AppendFormat("({0}),", updatedOrderId);
                    }
                    updatedOrderIdsParameter.Remove(updatedOrderIdsParameter.Length - 1, 1); //take off the last comma

                    orderHistoryEntries =
                        conn.Query<OrderHistory>(string.Format(@"DECLARE @OrderIds OrderIdsTableType
                                                INSERT INTO @OrderIds VALUES {0}
                                                select * from udf_GetOrderHistoryForOrderIds(@OrderIds)", updatedOrderIdsParameter)).ToList();

                    lineItems =
                        conn.Query<SearchResults.LineResult>(
                            "SELECT [Id], [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes], [PaidNotes] FROM vLineResults WHERE orderid in @updatedOrderIds",
                            new { updatedOrderIds }).ToList();
                    customAnswers =
                        conn.Query<SearchResults.CustomFieldResult>(
                            "SELECT [Id], [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults WHERE orderid in @updatedOrderIds",
                            new { updatedOrderIds }).ToList();

                    var orderInfo =
                        conn.Query<OrderTrackingDto>(
                            "select * from [vOrderTrackingIndex] WHERE orderid in @updatedOrderIds ORDER BY ActionDate DESC",
                            new {updatedOrderIds}).ToList();

                    orderTrackingEntities = ProcessTrackingEntities(orderInfo);
                }
            }

            if (updatedOrderIds.Any())
            {
                //Clear out existing lines and custom fields for the orders we are about to recreate
                _client.DeleteByQuery<SearchResults.LineResult>(Indices.Index(IndexHelper.GetIndexName(Indexes.LineItems)), Types.All, q=> q.Query(rq => rq.Terms(t => t.Field(f => f.OrderId).Terms(updatedOrderIds))));

                _client.DeleteByQuery<SearchResults.CustomFieldResult>(Indices.Index(IndexHelper.GetIndexName(Indexes.CustomAnswers)), Types.All, q => q.Query(rq => rq.Terms(t => t.Field(f => f.OrderId).Terms(updatedOrderIds))));

                _client.DeleteByQuery<OrderTrackingEntity>(Indices.Index(IndexHelper.GetIndexName(Indexes.OrderTracking)), Types.All, q => q.Query(rq => rq.Terms(t => t.Field(f => f.OrderId).Terms(updatedOrderIds))));

                WriteIndex(orderHistoryEntries, Indexes.OrderHistory, e => e.OrderId, recreate: false);
                WriteIndex(lineItems, Indexes.LineItems, recreate: false);
                WriteIndex(customAnswers, Indexes.CustomAnswers, recreate: false);
                WriteIndex(orderTrackingEntities, Indexes.OrderTracking, o => o.OrderId, recreate: false);
            }
        }

        /// <summary>
        /// return just the orders within the given date ranges
        /// </summary>
        /// <param name="orderids"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="startLastActionDate"></param>
        /// <param name="endLastActionDate"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public IndexedList<OrderHistory> GetOrderHistory(int[] orderids, DateTime? startDate, DateTime? endDate,
            DateTime? startLastActionDate, DateTime? endLastActionDate, string statusId)
        {
            var filters = new List<QueryContainer>();

            if (!endLastActionDate.HasValue)
            {
                endLastActionDate = DateTime.UtcNow.AddDays(1);
            }

            if (startLastActionDate.HasValue)
            {
                filters.Add(
                    Query<OrderHistory>.DateRange(
                        o =>
                            o.Field(x => x.LastActionDate)
                                .GreaterThanOrEquals(startLastActionDate.Value)
                                .LessThanOrEquals(endLastActionDate)));
            }

            if (startDate.HasValue)
            {
                filters.Add(Query<OrderHistory>.DateRange(o => o.Field(x => x.DateCreated).GreaterThanOrEquals(startDate)));
            }

            if (endDate.HasValue)
            {
                filters.Add(Query<OrderHistory>.DateRange(o => o.Field(x => x.DateCreated).LessThanOrEquals(endDate)));
            }
            if (!string.IsNullOrWhiteSpace(statusId))
            {
                filters.Add(Query<OrderHistory>.Term(a => a.StatusId, statusId.ToLower()));
            }

            var orders = _client.Search<OrderHistory>(
                s => s.Index(IndexHelper.GetIndexName(Indexes.OrderHistory))
                    .Size(orderids.Length > MaxReturnValues ? MaxReturnValues : orderids.Length)
                    .Query(q => q.Bool(b => b.Must(filters.ToArray())))
                    .PostFilter(f => f.ConstantScore(c => c.Filter(x => x.Terms(t => t.Field(q => q.OrderId).Terms(orderids)))))); //used to be .Query(f => f.And(filters.ToArray())));.  Now boolean must query/filter

            return new IndexedList<OrderHistory>
            {
                Results = orders.Hits.Select(h => h.Source).ToList(),
                LastModified = DateTime.UtcNow.ToPacificTime().AddMinutes(-5).ToLocalTime()
            };
        }

        public DateTime LastModified(Indexes index)
        {
            //TODO: no idea if this will work
            var indexName = IndexHelper.GetIndexName(index);

            return Convert.ToDateTime(_client.IndicesStats(indexName).Indices[indexName].Total.Indexing.Time);
        }

        public int NumRecords(Indexes index)
        {
            var indexName = IndexHelper.GetIndexName(index);

            // hopefully this works despite count not having a generic option.  I overwrote the index inside the action instead.
            return (int) _client.Count<OrderHistory>(x => x.Index(indexName)).Count;
        }

        public ElasticClient GetIndexClient()
        {
            return _client;
        }

        public void UpdateCommentsIndex()
        {
            var lastUpdate = DateTime.UtcNow.ToPacificTime().Subtract(TimeSpan.FromMinutes(10)); //10 minutes ago.

            IEnumerable<SearchResults.CommentResult> comments;

            using (var conn = _dbService.GetConnection())
            {
                comments =
                    conn.Query<SearchResults.CommentResult>(
                        "SELECT [Id], [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults where DateCreated > @lastUpdate", new { lastUpdate });
            }

            WriteIndex(comments, Indexes.Comments, recreate: false);
        }


        private IEnumerable<OrderTrackingEntity> ProcessTrackingEntities(IEnumerable<OrderTrackingDto> orderTrackingDtos)
        {
            //do work
            string[] completeList = {"completed", "cancelled", "denied"};
            var ordersWithTracking = from o in orderTrackingDtos
                                     group o by o.OrderId
                                         into orders
                                         select new { OrderId = orders.Key, TrackingInfo = orders.ToList() };
            var entities = new List<OrderTrackingEntity>();
            foreach (var order in ordersWithTracking)
            {
                var lastTrackingItem = order.TrackingInfo.First();
                var orderCompleted = order.TrackingInfo.FirstOrDefault(x => x.TrackingStatusComplete && completeList.Contains(x.Description) );
                var orderApprove = order.TrackingInfo.FirstOrDefault(x => x.Description == "approved" && x.TrackingStatusCode == "AP");
                var orderAccountManager =
                    order.TrackingInfo.FirstOrDefault(
                        x => x.Description == "approved" && x.TrackingStatusCode == "AM");


                var obj =
                    new OrderTrackingEntity
                    {
                        OrderId = order.OrderId,
                        OrderCreated = lastTrackingItem.OrderCreated,
                        WorkgroupId = lastTrackingItem.WorkgroupId,
                        WorkgroupName = lastTrackingItem.WorkgroupName,
                        IsComplete = lastTrackingItem.IsComplete,
                        StatusCode = lastTrackingItem.CurrentStatusCodeId,
                        Status = lastTrackingItem.CurrentStatusCode,
                        MinutesToCompletion =
                            orderCompleted == null
                                ? (double?)null
                                : (orderCompleted.ActionDate - lastTrackingItem.OrderCreated).TotalMinutes,
                        MinutesToApprove =
                            orderApprove == null
                                ? (double?)null
                                : (orderApprove.ActionDate - lastTrackingItem.OrderCreated).TotalMinutes,
                        ApproverName =
                            orderApprove == null
                                ? ""
                                : orderApprove.UserName,
                        ApproverId =
                            orderApprove == null
                                ? ""
                                : orderApprove.UserId,
                        MinutesToAccountManagerComplete =
                            orderAccountManager == null || orderApprove == null
                                ? (double?)null
                                : (orderAccountManager.ActionDate - orderApprove.ActionDate).TotalMinutes,
                        AccountManagerName =
                            orderAccountManager == null
                                ? ""
                                : orderAccountManager.UserName,
                        AccountManagerId =
                            orderAccountManager == null
                                ? ""
                                : orderAccountManager.UserId,
                        MinutesToPurchaserComplete =
                            orderCompleted == null || orderAccountManager == null
                                ? (double?)null
                                : (orderCompleted.ActionDate - orderAccountManager.ActionDate).TotalMinutes,
                        PurchaserName =
                            orderCompleted == null
                                ? ""
                                : orderCompleted.UserName,
                        PurchaserId =
                            orderCompleted == null
                                ? ""
                                : orderCompleted.UserId
                    };
                entities.Add(obj);

            }

            return entities;
        } 

        void WriteIndex<T>(string sqlSelect, Indexes indexes, Func<T, object> idAccessor = null, bool recreate = true) where T : class
        {
            IEnumerable<T> entities;

            using (var conn = _dbService.GetConnection())
            {
                entities = conn.Query<T>(sqlSelect);
            }

            WriteIndex(entities, indexes, idAccessor, recreate);
        }

        void WriteIndex<T>(IEnumerable<T> entities, Indexes indexes, Func<T, object> idAccessor = null, bool recreate = true) where T : class
        {
            if (entities == null)
            {
                return;
            }

            var index = IndexHelper.GetIndexName(indexes);
            
            if (recreate) //TODO: might have to check to see if index exists first time
            {
                _client.DeleteIndex(index);
                _client.CreateIndex(index);
            }

            var batches = entities.Partition(5000).ToArray(); //split into batches of up to 5000

            foreach (var batch in batches)
            {
                var bulkOperation = new BulkDescriptor();

                foreach (var item in batch)
                {
                    T localItem = item;

                    if (idAccessor == null) //if null let elasticsearch set the id
                    {
                        bulkOperation.Index<T>(b => b.Document(localItem).Index(index));
                    }
                    else
                    {
                        var id = idAccessor(localItem); //Be tricky so we can handle number and string ids

                        if (id is int)
                        {
                            bulkOperation.Index<T>(b => b.Document(localItem).Id((int) id).Index(index));
                        }
                        else
                        {
                            bulkOperation.Index<T>(b => b.Document(localItem).Id(id.ToString()).Index(index));
                        }
                    }
                }

                _client.Bulk(_ => bulkOperation);
            }
        }
    }

    public class OrderTrackingDto
    {
       
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Description { get; set; }
        public DateTime ActionDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string TrackingStatusCode { get; set; }
        public bool TrackingStatusComplete { get; set; }
        public int WorkgroupId { get; set; }
        public DateTime OrderCreated { get; set; }
        public string WorkgroupName { get; set; }
        public bool IsComplete { get; set; }
        public string CurrentStatusCode { get; set; }
        public string CurrentStatusCodeId { get; set; }
    }

    
    public class OrderTrackingEntity
    {
        public int OrderId { get; set; }
        public int WorkgroupId { get; set; }
        public string WorkgroupName { get; set; }
        public DateTime OrderCreated { get; set; }
        public double? MinutesToCompletion { get; set; }
        public double? MinutesToApprove { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        public double? MinutesToAccountManagerComplete { get; set; }
        public string AccountManagerName { get; set; }
        public string AccountManagerId { get; set; }
        public double? MinutesToPurchaserComplete { get; set; }
        public string PurchaserName { get; set; }
        public string PurchaserId { get; set; }
        public bool IsComplete { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
    }

    public class OrderTrackingAggregation 
    {
        public IList<OrderTrackingEntity> OrderTrackingEntities { get; set; }
        public double? AverageTimeToCompletion { get; set; }
        public double? AverageTimeToApprover { get; set; }
        public double? AverageTimeToAccountManagers { get; set; }
        public double? AverageTimeToPurchaser { get; set; }

    }

    public class OrderTrackingAggregationByRole
    {
        public IList<OrderTrackingEntity> OrderTrackingEntities { get; set; }
        public double? AverageTimeToRoleComplete { get; set; }
        public IList<double[]>  PercentilesForRole { get; set; }
        public string[] NamesInRole { get; set; }
    }

    

    public class OrderTrackingByRoleAggregation
    {
        // List of kerbs and percentiles
        public IList<OrderTrackingEntity> OrderTrackingEntities { get; set; }
        public double? AverageTimeForRole { get; set; }
    }

    public static class IndexHelper
    {
        public static string GetIndexName(Indexes indexes)
        {
            return string.Format("opp-{0}", indexes.ToNullSafeString().ToLowerInvariant());
        }
    }

    public class IndexedList<T>
    {
        public List<T> Results { get; set; }
        public DateTime LastModified { get; set; }
    }

    public enum Indexes
    {
        Accounts,
        Buildings,
        Comments,
        Commodities,
        CustomAnswers,
        LineItems,
        OrderHistory,
        OrderTracking,
        Vendors
    }

    public enum IndexOptions
    {
        Recreate, //Delete all documents and re-create the index
        UpdateOrder, //Update documents by first removing all matching orderIds and then adding in given documents
        UpdateItem //Update documents by removing item ids then adding in the given documents
    }

}