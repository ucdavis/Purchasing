using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;

namespace Purchasing.Core.Services
{
    public interface IIndexService
    {

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
    public class ElasticSearchIndexService : IIndexService
    {
        private readonly IDbService _dbService;
        private readonly SearchBackendSet _backends;
        private readonly IOrderIndexSource _orders;
        private const int CreateIndexQueryTimeout = 60 * 5;

        public ElasticSearchIndexService(IDbService dbService, SearchBackendSet backends)
            : this(dbService, backends, new SqlOrderIndexSource(dbService)) { }

        internal ElasticSearchIndexService(IDbService dbService, SearchBackendSet backends, IOrderIndexSource orders)
        {
            _dbService = dbService;
            _backends = backends;
            _orders = orders;
        }

        public void CreateAccountsIndex() => WriteIndex<Account>("SELECT [Id], [Name] FROM vAccounts WHERE [IsActive] = 1", Indexes.Accounts);
        public void CreateBuildingsIndex() => WriteIndex<Building>("SELECT [Id], [BuildingName] FROM vBuildings", Indexes.Buildings);
        public void CreateCommentsIndex() => WriteIndex<SearchResults.CommentResult>(SqlOrderIndexSource.CommentsSql, Indexes.Comments);
        public void CreateCommoditiesIndex() => WriteIndex<Commodity>("SELECT [Id], [Name] FROM vCommodities WHERE [IsActive] = 1", Indexes.Commodities);
        public void CreateCustomAnswersIndex() => WriteIndex<SearchResults.CustomFieldResult>(SqlOrderIndexSource.AnswersSql, Indexes.CustomAnswers);
        public void CreateLineItemsIndex() => WriteIndex<SearchResults.LineResult>(SqlOrderIndexSource.LinesSql, Indexes.LineItems);
        public void CreateVendorsIndex() => WriteIndex<Vendor>("SELECT [Id], [Name] FROM vVendors WHERE [IsActive] = 1", Indexes.Vendors);

        public void CreateHistoricalOrderIndex() => WriteIndex<OrderHistory>(
            "DECLARE @OrderIds OrderIdsTableType; INSERT INTO @OrderIds SELECT Id FROM Orders; SELECT * FROM udf_GetOrderHistoryForOrderIds(@OrderIds)",
            Indexes.OrderHistory);

        public void CreateTrackingIndex()
        {
            using var connection = _dbService.GetConnection();
            var tracking = connection.Query<OrderTrackingDto>(
                "SELECT * FROM vOrderTrackingIndex ORDER BY ActionDate DESC", commandTimeout: CreateIndexQueryTimeout).ToList();
            WriteAll(ProcessTrackingEntities(tracking).ToList(), Indexes.OrderTracking, true);
        }

        public void UpdateOrderIndexes()
        {
            var ids = _orders.ChangedOrderIds(DateTime.UtcNow.ToPacificTime().AddMinutes(-10));
            var failures = new IndexingFailures();
            foreach (var partition in ids.Distinct().Partition(1000))
            {
                var batchIds = partition.ToArray();
                failures.Attempt(() =>
                {
                    // Extract each batch once. Finish its writes before moving to the next batch.
                    var batch = _orders.ReadBatch(batchIds);
                    foreach (var target in _backends.Writers)
                    {
                        if (batch.History != null) failures.Attempt(() => target.Reconcile(Indexes.OrderHistory, batchIds, batch.History), $"{target.Name} order history");
                        if (batch.Lines != null) failures.Attempt(() => target.Reconcile(Indexes.LineItems, batchIds, batch.Lines), $"{target.Name} lines");
                        if (batch.Answers != null) failures.Attempt(() => target.Reconcile(Indexes.CustomAnswers, batchIds, batch.Answers), $"{target.Name} answers");
                        if (batch.Tracking != null) failures.Attempt(() => target.Reconcile(Indexes.OrderTracking, batchIds, batch.Tracking), $"{target.Name} tracking");
                        if (batch.Comments != null) failures.Attempt(() => target.Reconcile(Indexes.Comments, batchIds, batch.Comments), $"{target.Name} comments");
                    }
                    batch.Failures.ThrowIfAny();
                }, "extract/index orders", orderIds: batchIds);
            }
            failures.ThrowIfAny();
        }

        public void UpdateCommentsIndex() => WriteAll(
            _orders.RecentComments(DateTime.UtcNow.ToPacificTime().AddMinutes(-10)), Indexes.Comments, false);

        public IndexedList<OrderHistory> GetOrderHistory(int[] orderids, DateTime? startDate, DateTime? endDate,
            DateTime? startLastActionDate, DateTime? endLastActionDate, string statusId) =>
            new ElasticSearchService(_backends).GetOrderHistory(orderids, startDate, endDate, startLastActionDate, endLastActionDate, statusId);

        // Without recovery tracking, a reliable freshness checkpoint is unavailable.
        public DateTime LastModified(Indexes index) => DateTime.MinValue;
        public int NumRecords(Indexes index) => _backends.Reader.Count(index);

        private void WriteIndex<T>(string sql, Indexes index) where T : class
        {
            using var connection = _dbService.GetConnection();
            WriteAll(connection.Query<T>(sql, commandTimeout: CreateIndexQueryTimeout).ToList(), index, true);
        }

        private void WriteAll<T>(IEnumerable<T> documents, Indexes index, bool recreate) where T : class
        {
            var materialized = documents.ToList();
            var failures = new IndexingFailures();
            foreach (var target in _backends.Writers)
                failures.Attempt(() => target.Write(index, materialized, recreate), $"{target.Name}/{index}");
            failures.ThrowIfAny();
        }

        internal static IEnumerable<OrderTrackingEntity> ProcessTrackingEntities(IEnumerable<OrderTrackingDto> orderTrackingDtos)
        {
            //do work
            string[] completeList = { "completed", "cancelled", "denied" };
            var ordersWithTracking = from o in orderTrackingDtos
                                     group o by o.OrderId
                                         into orders
                                     select new { OrderId = orders.Key, TrackingInfo = orders.ToList() };
            var entities = new List<OrderTrackingEntity>();
            foreach (var order in ordersWithTracking)
            {
                var lastTrackingItem = order.TrackingInfo.First();
                var orderCompleted = order.TrackingInfo.FirstOrDefault(x => x.TrackingStatusComplete && completeList.Contains(x.Description));
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
        public IList<double[]> PercentilesForRole { get; set; }
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
            return string.Format("opp-{0}", indexes.ToString().ToLowerInvariant());
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
