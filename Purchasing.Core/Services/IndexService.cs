using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper.Internal;
using Dapper;
using Nest;
using NHibernate.Hql.Ast.ANTLR;
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

        IndexedList<OrderHistory> GetOrderHistory(int[] orderids);
        DateTime LastModified(Indexes index);
        int NumRecords(Indexes index);
        void UpdateCommentsIndex();
        ElasticClient GetIndexClient();
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

        public ElasticSearchIndexService(IDbService dbService)
        {
            _dbService = dbService;

            var settings =
                new ConnectionSettings(new Uri("https://ekgrfdtq8y:o4jy2eyhdu@caesdo-8165237795.us-west-2.bonsai.io"),
                    "prepurchasing");
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
            IList<OrderTrackingDto> orderTrackings;
            using (var conn = _dbService.GetConnection())
            {
                orderTrackings =
                    conn.Query<OrderTrackingDto>(@"select OrderTracking.Id, OrderTracking.OrderId, Description, OrderTracking.DateCreated as ActionDate, 
                UserId, users.FirstName + ' ' + users.LastName as userName, OrderTracking.OrderStatusCodeId as TrackingStatusCode, 
				Orders.WorkgroupId, workgroups.Name as workgroupName, Orders.DateCreated as OrderCreated,
				orders.OrderStatusCodeId as CurrentStatusCodeId, OrderStatusCodes.Name as CurrentStatusCode
                from OrderTracking
	            inner join Orders on OrderId = Orders.Id
				LEFT outer join Users ON OrderTracking.UserId = Users.Id
				LEFT OUTER join Workgroups ON orders.WorkgroupId=workgroups.Id 
				inner join OrderStatusCodes ON orders.OrderStatusCodeId = OrderStatusCodes.Id
                WHERE orderid < 50
                ORDER BY OrderTracking.DateCreated DESC", Indexes.OrderTracking).ToList();

                //do work
                var ordersWithTracking = from o in orderTrackings
                    group o by o.OrderId
                    into orders
                    select new {OrderId = orders.Key, TrackingInfo = orders.ToList()};
                var entities = new List<OrderTrackingEntity>();
                foreach (var order in ordersWithTracking)
                {
                    var orderCreated = order.TrackingInfo.First();
                    var orderCompleted = order.TrackingInfo.FirstOrDefault(x => x.TrackingStatusCode == "CP" || x.TrackingStatusCode == "CN");
                    var orderApprove = order.TrackingInfo.FirstOrDefault(x => x.Description == "approved" && x.TrackingStatusCode == "AP");
                    var orderAccountManager =
                        order.TrackingInfo.FirstOrDefault(x => x.Description == "approved" && x.TrackingStatusCode == "AM");


                    var obj =
                        new OrderTrackingEntity
                        {
                            OrderId = order.OrderId,
                            OrderCreated = orderCreated.OrderCreated,
                            WorkgroupId = orderCreated.WorkgroupId,
                            WorkgroupName = orderCreated.WorkgroupName,
                            IsComplete =
                                orderCreated.CurrentStatusCodeId == "CP" || orderCreated.CurrentStatusCodeId == "CN" || orderCreated.CurrentStatusCodeId == "OC" || orderCreated.CurrentStatusCodeId == "OD",
                            StatusCode = orderCreated.CurrentStatusCodeId,
                            Status = orderCreated.CurrentStatusCode,
                            MinutesToCompletion =
                                orderCompleted == null
                                    ? 0
                                    : (orderCompleted.ActionDate - orderCreated.OrderCreated).TotalMinutes,
                            MinutesToApprove =
                                orderApprove == null
                                    ? 0
                                    : (orderApprove.ActionDate - orderCreated.OrderCreated).TotalMinutes,
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
                                    ? 0
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
                                    ? 0
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

                WriteIndex(entities, Indexes.OrderTracking);
            }
        }

        public void UpdateOrderIndexes()
        {
            var lastUpdate = DateTime.UtcNow.ToPacificTime().Subtract(TimeSpan.FromMinutes(10)); //10 minutes ago.

            IEnumerable<OrderHistory> orderHistoryEntries = null;
            IEnumerable<SearchResults.LineResult> lineItems = null;
            IEnumerable<SearchResults.CustomFieldResult> customAnswers = null;

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
                }
            }

            if (updatedOrderIds.Any())
            {
                //Clear out existing lines and custom fields for the orders we are about to recreate
                _client.DeleteByQuery<SearchResults.LineResult>(
                    q => q.Index(IndexHelper.GetIndexName(Indexes.LineItems)).Query(rq => rq.Terms(f => f.OrderId, updatedOrderIds)));

                _client.DeleteByQuery<SearchResults.CustomFieldResult>(
                    q =>
                        q.Index(IndexHelper.GetIndexName(Indexes.CustomAnswers))
                            .Query(rq => rq.Terms(f => f.OrderId, updatedOrderIds)));

                WriteIndex(orderHistoryEntries, Indexes.OrderHistory, e => e.OrderId, recreate: false);
                WriteIndex(lineItems, Indexes.LineItems, recreate: false);
                WriteIndex(customAnswers, Indexes.CustomAnswers, recreate: false);
            }
        }

        public IndexedList<OrderHistory> GetOrderHistory(int[] orderids)
        {
            var orders = _client.Search<OrderHistory>(
                s => s.Index(IndexHelper.GetIndexName(Indexes.OrderHistory))
                    .Size(orderids.Length)
                    .Filter(q => q.Terms(o => o.OrderId, orderids)));

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
            return
                Convert.ToDateTime(_client.IndicesStats(i => i.Index(indexName)).Indices[indexName].Total.Indexing.Time);
        }

        public int NumRecords(Indexes index)
        {
            var indexName = IndexHelper.GetIndexName(index);
            return (int) _client.Status(i => i.Index(indexName)).Indices[indexName].IndexDocs.NumberOfDocs;
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

            var batches = entities.Partition(500).ToArray(); //split into batches of up to 500

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
        public double MinutesToCompletion { get; set; }
        public double MinutesToApprove { get; set; }
        public string ApproverName { get; set; }
        public string ApproverId { get; set; }
        public double MinutesToAccountManagerComplete { get; set; }
        public string AccountManagerName { get; set; }
        public string AccountManagerId { get; set; }
        public double MinutesToPurchaserComplete { get; set; }
        public string PurchaserName { get; set; }
        public string PurchaserId { get; set; }
        public bool IsComplete { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
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