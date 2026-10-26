using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class SearchBackendIntegrationTests
    {
        [TestMethod]
        [TestCategory("SearchIntegration")]
        public void DualWritesAndReadSwitchesWorkWithBothRealServers()
        {
            var bonsai = Environment.GetEnvironmentVariable("PURCHASING_TEST_ELASTIC7_URL");
            var cloud = Environment.GetEnvironmentVariable("PURCHASING_TEST_ELASTIC9_URL");
            if (string.IsNullOrEmpty(bonsai) || string.IsNullOrEmpty(cloud))
                Assert.Inconclusive("Both disposable local Elasticsearch instances are required.");
            Assert.IsTrue(new Uri(bonsai).IsLoopback && new Uri(cloud).IsLoopback);
            var prefix = "opp-test-" + Guid.NewGuid().ToString("N");
            SearchBackendSet Select(string reader) => new SearchBackendSet(DualIndexingTests.Config(
                ("Search:ReadBackend", reader), ("Search:WriteTargets", "Both"),
                ("Search:Bonsai:Url", bonsai), ("Search:ElasticCloud:Url", cloud),
                ("Search:Bonsai:IndexPrefix", prefix), ("Search:ElasticCloud:IndexPrefix", prefix)));
            var initial = Select("Bonsai");
            try
            {
                var source = new DualIndexingTests.FakeOrderSource(2);
                new ElasticSearchIndexService(null, initial, source).UpdateOrderIndexes();
                Assert.AreEqual(1, source.Reads.Count);
                foreach (var target in initial.Writers) target.Request("POST", $"/{prefix}-*/_refresh");
                foreach (var reader in new[] { "Bonsai", "ElasticCloud", "Bonsai" })
                {
                    var service = new ElasticSearchIndexService(null, Select(reader));
                    Assert.AreEqual(2, service.GetOrderHistory(new[] { 1, 2 }, null, null, null, null, null).Results.Count);
                }
            }
            finally
            {
                foreach (var target in initial.Writers)
                {
                    var indexes = target.Request("GET", $"/{prefix}-*", allowNotFound: true);
                    if (indexes != null)
                        foreach (var index in indexes.Properties()) target.Request("DELETE", "/" + index.Name);
                }
            }
        }

        [DataTestMethod]
        [DataRow("Bonsai", "PURCHASING_TEST_ELASTIC7_URL")]
        [DataRow("ElasticCloud", "PURCHASING_TEST_ELASTIC9_URL")]
        [TestCategory("SearchIntegration")]
        public void SearchReportsAndRepeatableIndexingWorkOnRealServer(string name, string variable)
        {
            var url = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(url)) Assert.Inconclusive($"Set {variable} to a disposable local Elasticsearch instance.");
            Assert.IsTrue(new Uri(url).IsLoopback, "Integration tests only write to loopback instances.");
            var prefix = "opp-test-" + Guid.NewGuid().ToString("N");
            var configuration = DualIndexingTests.Config(("Search:ReadBackend", name), ("Search:WriteTargets", name),
                ($"Search:{name}:Url", url), ($"Search:{name}:IndexPrefix", prefix));
            var set = new SearchBackendSet(configuration);
            var backend = set.Reader;
            var search = new ElasticSearchService(set);
            var date = new DateTime(2012, 5, 10);
            try
            {
                backend.Write(Indexes.OrderHistory, new[]
                {
                    new OrderHistory { OrderId = 42, Id = 900, WorkgroupId = 7, RequestNumber = "OLD-42", Justification = "Office paper", StatusId = "CP", DateCreated = date, LastActionDate = date, TotalAmount = 12.34m },
                    new OrderHistory { OrderId = 43, Id = 901, WorkgroupId = 8, RequestNumber = "PRIVATE-43", Justification = "Office paper", StatusId = "CP", DateCreated = date.AddDays(1), LastActionDate = date.AddDays(1) }
                }, recreate: true);
                backend.Write(Indexes.LineItems, new[]
                {
                    new SearchResults.LineResult { Id = 1, OrderId = 42, Description = "Office paper", Quantity = 1.25m },
                    new SearchResults.LineResult { Id = 2, OrderId = 43, Description = "Office paper", Quantity = 2 }
                }, recreate: true);
                backend.Write(Indexes.Comments, new[]
                {
                    new SearchResults.CommentResult { Id = 1, OrderId = 42, Text = "Office paper", DateCreated = date },
                    new SearchResults.CommentResult { Id = 2, OrderId = 43, Text = "Office paper", DateCreated = date.AddDays(1) }
                }, recreate: true);
                backend.Write(Indexes.CustomAnswers, new[] { new SearchResults.CustomFieldResult { Id = 1, OrderId = 42, Question = "What?", Answer = "Office paper" } }, recreate: true);
                backend.Write(Indexes.Accounts, new[] { new Account { Id = "A1", Name = "Office paper" } }, recreate: true);
                backend.Write(Indexes.Buildings, new[] { new Building { Id = "B1", BuildingName = "Office paper" } }, recreate: true);
                // Existing Bonsai installations start with concrete index names, not aliases.
                backend.Write(Indexes.Commodities, new[] { new Commodity { Id = "C0", Name = "Previous commodity" } });
                Assert.AreEqual(DateTime.MinValue, backend.LastModified(Indexes.Commodities));
                backend.Write(Indexes.Commodities, new[] { new Commodity { Id = "C1", Name = "Office paper" } }, recreate: true);
                Assert.AreEqual(DateTime.MinValue, backend.LastModified(Indexes.Commodities));
                backend.Write(Indexes.Vendors, new[] { new Vendor { Id = "V1", Name = "Office paper" } }, recreate: true);
                backend.Write(Indexes.OrderTracking, new[] { new OrderTrackingEntity
                {
                    OrderId = 42, WorkgroupId = 7, OrderCreated = date, Status = "Complete", IsComplete = true,
                    MinutesToCompletion = 4320, MinutesToApprove = 1440, MinutesToAccountManagerComplete = 1440,
                    MinutesToPurchaserComplete = 1440, ApproverId = "jane", AccountManagerId = "pat", PurchaserId = "lee"
                } }, recreate: true);

                CollectionAssert.AreEqual(new[] { 42 }, search.SearchOrders("paper", new[] { 42 }).Select(x => x.Id).ToArray());
                CollectionAssert.AreEqual(new[] { 43, 42 }, search.SearchOrders("paper", new[] { 42, 43 }).Select(x => x.Id).ToArray());
                Assert.AreEqual(0, search.SearchOrders("paper", Array.Empty<int>()).Count);
                Assert.AreEqual(42, search.SearchLineItems("paper", new[] { 42 }).Single().OrderId);
                Assert.AreEqual(1.25m, search.SearchLineItems("paper", new[] { 42 }).Single().Quantity);
                Assert.AreEqual(42, search.SearchComments("paper", new[] { 42 }).Single().OrderId);
                Assert.AreEqual(42, search.GetLatestComments(1, new[] { 42 }).Single().OrderId);
                Assert.AreEqual(42, search.SearchCustomFieldAnswers("paper", new[] { 42 }).Single().OrderId);
                Assert.AreEqual(1, search.SearchAccounts("paper").Count);
                Assert.AreEqual(1, search.SearchBuildings("paper").Count);
                Assert.AreEqual(1, search.SearchCommodities("paper").Count);
                Assert.AreEqual(1, search.SearchVendors("paper").Count);

                var history = search.GetOrderHistory(new[] { 42, 43 }, date.AddDays(-1), date, date.AddDays(-1), date, "CP").Results.Single();
                Assert.AreEqual(42, history.OrderId);
                Assert.AreEqual(42, history.Id);
                Assert.AreEqual(12.34m, history.TotalAmount);
                var workgroups = new[] { new Workgroup { Id = 7 } };
                Assert.AreEqual(42, search.GetOrdersByWorkgroups(workgroups, date.AddDays(-1), date.AddDays(2)).Single().OrderId);
                var aggregation = search.GetOrderTrackingEntities(workgroups, date.AddDays(-1), date.AddDays(2));
                Assert.AreEqual(4320d, aggregation.AverageTimeToCompletion);
                Assert.AreEqual(1440d, aggregation.AverageTimeToApprover);
                foreach (var role in new[] { "Approver", "AccountManager", "Purchaser" })
                {
                    var byRole = search.GetOrderTrackingEntitiesByRole(workgroups, date.AddDays(-1), date.AddDays(2), role, 1000);
                    Assert.AreEqual(1d, byRole.AverageTimeToRoleComplete);
                    Assert.AreEqual(1, byRole.NamesInRole.Length);
                    CollectionAssert.AreEqual(new[] { 1d, 1d, 1d, 1d, 1d }, byRole.PercentilesForRole.Single());
                }

                // Above Elasticsearch's default 65,536 terms limit, with only one authorized match.
                var largePermissions = Enumerable.Range(100000, 70000).Append(42).ToArray();
                Assert.AreEqual(42, search.SearchOrders("paper", largePermissions).Single().Id);

                // Repeated updates must see unrefreshed prior writes, remove old children,
                // retain another order's documents, and avoid duplicate tracking documents.
                var replacement = new[] { new SearchResults.LineResult { Id = 3, OrderId = 42, Description = "Replacement paper" } };
                backend.Reconcile(Indexes.LineItems, new[] { 42 }, replacement);
                backend.Reconcile(Indexes.LineItems, new[] { 42 }, replacement);
                backend.Request("POST", $"/{backend.IndexName(Indexes.LineItems)}/_refresh");
                Assert.AreEqual(2, backend.Count(Indexes.LineItems));
                Assert.AreEqual(3, search.SearchLineItems("paper", new[] { 42 }).Single().Id);
                backend.Reconcile(Indexes.LineItems, new[] { 42 }, Array.Empty<SearchResults.LineResult>());
                Assert.AreEqual(1, backend.Count(Indexes.LineItems));
                Assert.AreEqual(43, search.SearchLineItems("paper", new[] { 42, 43 }).Single().OrderId);
                backend.Reconcile(Indexes.OrderTracking, new[] { 42 }, new[] { new OrderTrackingEntity { OrderId = 42, WorkgroupId = 7, OrderCreated = date } });
                backend.Request("POST", $"/{backend.IndexName(Indexes.OrderTracking)}/_refresh");
                Assert.AreEqual(1, backend.Count(Indexes.OrderTracking));
                Assert.ThrowsExactly<AggregateException>(() => backend.Write(Indexes.OrderTracking,
                    new[] { new OrderTrackingEntity { OrderId = 999, MinutesToCompletion = double.NaN } }, recreate: true));
                Assert.AreEqual(1, backend.Count(Indexes.OrderTracking), "A failed replacement must leave the active index available.");

                backend.Write(Indexes.Commodities, new[] { new Commodity { Id = "C2", Name = "Replacement paper" } }, recreate: true);
                Assert.AreEqual(1, search.SearchCommodities("replacement").Count);
                Assert.ThrowsExactly<SearchBackendException>(() => search.SearchOrders("\"unterminated", new[] { 42 }));
            }
            finally
            {
                var indexes = backend.Request("GET", $"/{prefix}-*", allowNotFound: true);
                if (indexes != null)
                    foreach (var index in indexes.Properties()) backend.Request("DELETE", "/" + index.Name);
            }
        }
    }
}
