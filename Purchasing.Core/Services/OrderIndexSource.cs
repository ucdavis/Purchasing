using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Purchasing.Core.Queries;

namespace Purchasing.Core.Services
{
    internal interface IOrderIndexSource
    {
        int[] ChangedOrderIds(DateTime since);
        OrderIndexBatch ReadBatch(int[] orderIds);
        IEnumerable<SearchResults.CommentResult> RecentComments(DateTime since);
    }

    internal sealed class OrderIndexBatch
    {
        public List<OrderHistory> History { get; set; }
        public List<SearchResults.LineResult> Lines { get; set; }
        public List<SearchResults.CustomFieldResult> Answers { get; set; }
        public List<OrderTrackingEntity> Tracking { get; set; }
        public List<SearchResults.CommentResult> Comments { get; set; }
        public IndexingFailures Failures { get; } = new IndexingFailures();
    }

    internal sealed class SqlOrderIndexSource : IOrderIndexSource
    {
        internal const string LinesSql = "SELECT [Id], [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes], [PaidNotes] FROM vLineResults";
        internal const string AnswersSql = "SELECT [Id], [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults";
        internal const string CommentsSql = "SELECT [Id], [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults";
        private readonly IDbService _db;
        public SqlOrderIndexSource(IDbService db) { _db = db; }

        public int[] ChangedOrderIds(DateTime since)
        {
            using var connection = _db.GetConnection();
            return connection.Query<int>("SELECT DISTINCT OrderId FROM OrderTracking WHERE DateCreated > @since", new { since }).ToArray();
        }

        public OrderIndexBatch ReadBatch(int[] orderIds)
        {
            using var connection = _db.GetConnection();
            // Values come only from integer SQL IDs and batches stay within SQL's 1,000-row VALUES limit.
            var values = string.Join(",", orderIds.Select(id => $"({id})"));
            var result = new OrderIndexBatch();
            result.History = ReadProjection(() => connection.Query<OrderHistory>(
                $"DECLARE @OrderIds OrderIdsTableType; INSERT INTO @OrderIds VALUES {values}; SELECT * FROM udf_GetOrderHistoryForOrderIds(@OrderIds)"),
                Indexes.OrderHistory, orderIds, result.Failures);
            result.Lines = ReadProjection(() => connection.Query<SearchResults.LineResult>(LinesSql + " WHERE OrderId IN @orderIds", new { orderIds }),
                Indexes.LineItems, orderIds, result.Failures);
            result.Answers = ReadProjection(() => connection.Query<SearchResults.CustomFieldResult>(AnswersSql + " WHERE OrderId IN @orderIds", new { orderIds }),
                Indexes.CustomAnswers, orderIds, result.Failures);
            result.Comments = ReadProjection(() => connection.Query<SearchResults.CommentResult>(CommentsSql + " WHERE OrderId IN @orderIds", new { orderIds }),
                Indexes.Comments, orderIds, result.Failures);
            result.Tracking = ReadProjection(() => ElasticSearchIndexService.ProcessTrackingEntities(connection.Query<OrderTrackingDto>(
                "SELECT * FROM vOrderTrackingIndex WHERE OrderId IN @orderIds ORDER BY ActionDate DESC", new { orderIds })),
                Indexes.OrderTracking, orderIds, result.Failures);
            return result;
        }

        private static List<T> ReadProjection<T>(Func<IEnumerable<T>> read, Indexes index, int[] orderIds, IndexingFailures failures)
        {
            List<T> documents = null;
            failures.Attempt(() =>
            {
                try { documents = read().ToList(); }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"SQL {index} extraction failed ({exception.GetType().Name}).");
                }
            }, $"SQL/{index} extraction", orderIds: orderIds);
            return documents;
        }

        public IEnumerable<SearchResults.CommentResult> RecentComments(DateTime since)
        {
            using var connection = _db.GetConnection();
            return connection.Query<SearchResults.CommentResult>(CommentsSql + " WHERE DateCreated > @since", new { since }).ToList();
        }
    }
}
