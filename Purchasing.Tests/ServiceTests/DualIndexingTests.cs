using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    [DoNotParallelize]
    public class DualIndexingTests
    {
        internal static IConfiguration Config(params (string Key, string Value)[] entries) =>
            new ConfigurationBuilder().AddInMemoryCollection(entries.ToDictionary(e => e.Key, e => e.Value)).Build();

        [TestMethod]
        public void DefaultsKeepBonsaiAndReadSelectionIsIndependent()
        {
            var defaults = new SearchBackendConfiguration(Config(("ElasticSearchUrl", "http://localhost:9200")));
            Assert.AreEqual(SearchBackend.Bonsai, defaults.ReadBackend);
            CollectionAssert.AreEqual(new[] { SearchBackend.Bonsai }, defaults.WriteTargets.ToArray());
            var selected = new SearchBackendConfiguration(Config(("ElasticSearchUrl", "http://localhost:9200"),
                ("Search:ReadBackend", "ElasticCloud"), ("Search:ElasticCloud:Url", "http://localhost:9201")));
            Assert.AreEqual(SearchBackend.ElasticCloud, selected.ReadBackend);
            CollectionAssert.AreEqual(new[] { SearchBackend.Bonsai }, selected.WriteTargets.ToArray());
        }

        [TestMethod]
        public void ConfigurationRejectsMissingTargetAndInvalidNamesWithoutLeakingCredentials()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new SearchBackendConfiguration(Config(
                ("ElasticSearchUrl", "http://localhost:9200"), ("Search:WriteTargets", "Both"))));
            Assert.ThrowsExactly<ArgumentException>(() => new SearchBackendConfiguration(Config(("Search:ReadBackend", "unknown"))));
            Assert.ThrowsExactly<ArgumentException>(() => new SearchBackendConfiguration(Config(
                ("ElasticSearchUrl", "http://localhost:9200"), ("Search:Bonsai:IndexPrefix", "shared/*"))));
            var error = Assert.ThrowsExactly<ArgumentException>(() => new SearchBackendConfiguration(Config(
                ("ElasticSearchUrl", "http://user:secret@localhost:9200/?secret=value"))));
            Assert.IsFalse(error.Message.Contains("secret"));
            var options = new SearchBackendConfiguration(Config(("ElasticSearchUrl", "http://user:p%40ss@localhost:9200")));
            Assert.AreEqual("p@ss", options.Endpoints[SearchBackend.Bonsai].Password);
            Assert.AreEqual("", options.Endpoints[SearchBackend.Bonsai].Url.UserInfo);
        }

        [TestMethod]
        public void MoreThan1000OrdersAreExtractedOnceAndAllBatchesReachBothTargetsAfterFailures()
        {
            var source = new FakeOrderSource(2001);
            var bonsai = new RecordingBackend("Bonsai") { FailingDocument = "3" };
            var cloud = new RecordingBackend("ElasticCloud");
            var service = new ElasticSearchIndexService(null, new SearchBackendSet(bonsai, bonsai, cloud), source);

            Assert.ThrowsExactly<AggregateException>(() => service.UpdateOrderIndexes());

            CollectionAssert.AreEqual(new[] { 1000, 1000, 1 }, source.Reads.Select(r => r.Length).ToArray());
            foreach (var backend in new[] { bonsai, cloud })
            {
                var history = backend.Documents.Where(d => d.Index.EndsWith("-orderhistory")).ToList();
                Assert.AreEqual(2001, history.Count);
                CollectionAssert.AreEquivalent(Enumerable.Range(1, 2001).Select(i => i.ToString()).ToArray(), history.Select(d => d.Id).ToArray());
                Assert.AreEqual(15, backend.Deletes.Count);
                Assert.IsTrue(backend.Documents.Any(d => d.Index.EndsWith("-comments") && d.Id == "2001"));
            }
        }

        [DataTestMethod]
        [DataRow("/_delete_by_query", 200, "{\"deleted\":999,\"failures\":[{\"reason\":\"private response detail\"}]}")]
        [DataRow("/_delete_by_query", 200, "{\"deleted\":999,\"version_conflicts\":1}")]
        [DataRow("/_delete_by_query", 200, "{\"timed_out\":true}")]
        [DataRow("/_delete_by_query", 503, "private response detail")]
        [DataRow("/_delete_by_query", 0, "")]
        [DataRow("/_refresh", 200, "{\"_shards\":{\"failed\":1}}")]
        [DataRow("/_refresh", 503, "private response detail")]
        [DataRow("/_refresh", 0, "")]
        public void ReconciliationFailuresStillWriteEveryIndexBatchAndDestination(string failingPath, int status, string response)
        {
            using var logs = new CapturedLogs();
            var bonsai = new RecordingBackend("Bonsai")
            {
                ResponseOverride = (method, path, body) => path.Contains(failingPath)
                    ? status == 0 ? throw new TimeoutException("private transport detail") : (status, response)
                    : null
            };
            var cloud = new RecordingBackend("ElasticCloud");
            var source = new FakeOrderSource(1001);
            var service = new ElasticSearchIndexService(null, new SearchBackendSet(bonsai, bonsai, cloud), source);
            Assert.ThrowsExactly<AggregateException>(() => service.UpdateOrderIndexes());
            CollectionAssert.AreEqual(new[] { 1000, 1 }, source.Reads.Select(r => r.Length).ToArray());
            foreach (var index in new[] { Indexes.OrderHistory, Indexes.LineItems, Indexes.CustomAnswers, Indexes.OrderTracking, Indexes.Comments })
            {
                foreach (var backend in new[] { bonsai, cloud })
                {
                    CollectionAssert.AreEqual(Enumerable.Range(1, 1001).Select(i => i.ToString()).ToArray(),
                        backend.Documents.Where(d => d.Index == backend.IndexName(index)).Select(d => d.Id).ToArray());
                    Assert.AreEqual(2, backend.Calls.Count(c => c.Path.StartsWith($"/{backend.IndexName(index)}/_delete_by_query")));
                }
                var operation = $"Bonsai/{index} {(failingPath == "/_refresh" ? "refresh" : "delete")}";
                var failures = logs.Events.Where(e => Operation(e) == operation).ToArray();
                Assert.AreEqual(2, failures.Length);
                CollectionAssert.AreEqual(Enumerable.Range(1, 1000).ToArray(), OrderIds(failures[0]));
                CollectionAssert.AreEqual(new[] { 1001 }, OrderIds(failures[1]));
            }
            AssertSafeLogs(logs);
        }

        [TestMethod]
        public void UnavailableDestinationDoesNotStopHealthyDestination()
        {
            using var logs = new CapturedLogs();
            var unavailable = new RecordingBackend("ElasticCloud") { ThrowOnRequest = true };
            var healthy = new RecordingBackend("Bonsai");
            var service = new ElasticSearchIndexService(null, new SearchBackendSet(healthy, unavailable, healthy), new FakeOrderSource(1));
            var exception = Assert.ThrowsExactly<AggregateException>(() => service.UpdateOrderIndexes());
            Assert.AreEqual(20, unavailable.Calls.Count);
            Assert.AreEqual(5, healthy.Documents.Count);
            Assert.IsFalse(exception.ToString().Contains("private transport detail"));
            var failures = logs.Events.Where(e => e.Properties.TryGetValue("OrderIds", out var ids) && ids is SequenceValue).ToArray();
            Assert.AreEqual(20, failures.Length);
            foreach (var failure in failures) CollectionAssert.AreEqual(new[] { 1 }, OrderIds(failure));
            AssertSafeLogs(logs);
        }

        [TestMethod]
        public void ReconciliationReportsBothDeleteAndReplacementFailures()
        {
            using var logs = new CapturedLogs();
            var backend = new RecordingBackend("Bonsai")
            {
                FailingDocument = "7",
                ResponseOverride = (method, path, body) => path.Contains("/_delete_by_query") ? (200, "{\"version_conflicts\":1}") : null
            };
            var failure = Assert.ThrowsExactly<AggregateException>(() => backend.Reconcile(Indexes.Comments, new[] { 42 },
                new[] { new SearchResults.CommentResult { Id = 7, OrderId = 42, Text = "private document body" } }));
            Assert.AreEqual(2, failure.Flatten().InnerExceptions.Count);
            Assert.AreEqual("7", backend.Documents.Single().Id);
            var bulkFailure = logs.Events.Single(e => Operation(e) == "Bonsai/test-comments bulk request");
            CollectionAssert.AreEqual(new object[] { 7 }, DocumentValues(bulkFailure, "DocumentId"));
            CollectionAssert.AreEqual(new object[] { 42 }, DocumentValues(bulkFailure, "OrderId"));
            AssertSafeLogs(logs);
        }

        [TestMethod]
        public void ReconciliationFailureLogsOrdersWithNoReplacementDocuments()
        {
            using var logs = new CapturedLogs();
            var backend = new RecordingBackend("Bonsai")
            {
                ResponseOverride = (method, path, body) => path.Contains("/_delete_by_query") ? (503, "private response detail") : null
            };
            Assert.ThrowsExactly<AggregateException>(() => backend.Reconcile(Indexes.Comments, new[] { 42, 43 },
                Array.Empty<SearchResults.CommentResult>()));
            var failure = logs.Events.Single(e => Operation(e) == "Bonsai/Comments delete");
            CollectionAssert.AreEqual(new[] { 42, 43 }, OrderIds(failure));
            Assert.AreEqual(0, backend.Documents.Count);
            AssertSafeLogs(logs);
        }

        [DataTestMethod]
        [DataRow(503, "private response detail", false)]
        [DataRow(503, "private response detail", true)]
        [DataRow(200, "{}", false)]
        [DataRow(200, "{}", true)]
        [DataRow(200, "{\"items\":[]}", false)]
        [DataRow(200, "{\"items\":[]}", true)]
        [DataRow(200, "private invalid JSON", false)]
        [DataRow(200, "private invalid JSON", true)]
        [DataRow(0, "", false)]
        [DataRow(0, "", true)]
        public void WholeBulkFailuresLogSubmittedIdentifiersAndContinueBatches(int status, string response, bool recreate)
        {
            using var logs = new CapturedLogs();
            var backend = new RecordingBackend("ElasticCloud")
            {
                ResponseOverride = (method, path, body) => path == "/_bulk"
                    ? status == 0 ? throw new TimeoutException("private transport detail") : (status, response)
                    : null
            };
            var documents = Enumerable.Range(1, 1001).Select(i => new SearchResults.CommentResult
            {
                Id = i, OrderId = 2000 + i, Text = "private document body"
            }).ToArray();

            Assert.ThrowsExactly<AggregateException>(() => backend.Write(Indexes.Comments, documents, recreate));

            Assert.AreEqual(2, backend.Calls.Count(c => c.Path == "/_bulk"));
            Assert.IsFalse(backend.Calls.Any(c => c.Path == "/_aliases"));
            var failures = logs.Events.Where(e => Operation(e)?.EndsWith(" bulk request") == true).ToArray();
            Assert.AreEqual(2, failures.Length);
            Assert.IsTrue(failures.All(e => Operation(e).StartsWith("ElasticCloud/test-comments")));
            CollectionAssert.AreEqual(Enumerable.Range(1, 1000).Select(i => (object)i).ToArray(), DocumentValues(failures[0], "DocumentId"));
            CollectionAssert.AreEqual(Enumerable.Range(2001, 1000).Select(i => (object)i).ToArray(), DocumentValues(failures[0], "OrderId"));
            CollectionAssert.AreEqual(new object[] { 1001 }, DocumentValues(failures[1], "DocumentId"));
            CollectionAssert.AreEqual(new object[] { 3001 }, DocumentValues(failures[1], "OrderId"));
            AssertSafeLogs(logs);
        }

        [TestMethod]
        public void RecentCommentFailureLogsIdentifiersAndStillWritesHealthyDestination()
        {
            using var logs = new CapturedLogs();
            var unavailable = new RecordingBackend("ElasticCloud")
            {
                ResponseOverride = (method, path, body) => path == "/_bulk" ? (503, "private response detail") : null
            };
            var healthy = new RecordingBackend("Bonsai");
            var source = new FakeOrderSource(0)
            {
                Comments = new[] { new SearchResults.CommentResult { Id = 7, OrderId = 42, Text = "private document body" } }
            };
            var service = new ElasticSearchIndexService(null, new SearchBackendSet(healthy, unavailable, healthy), source);
            Assert.ThrowsExactly<AggregateException>(() => service.UpdateCommentsIndex());
            Assert.AreEqual("7", healthy.Documents.Single().Id);
            var failure = logs.Events.Single(e => Operation(e) == "ElasticCloud/test-comments bulk request");
            CollectionAssert.AreEqual(new object[] { 7 }, DocumentValues(failure, "DocumentId"));
            CollectionAssert.AreEqual(new object[] { 42 }, DocumentValues(failure, "OrderId"));
            AssertSafeLogs(logs);
        }

        [TestMethod]
        public void SerializationFailuresLogIdentifiersWithoutPayloadsAndStillWriteHealthyDocuments()
        {
            var cases = new (Indexes Index, object Failed, object Healthy, object Id, object OrderId)[]
            {
                (Indexes.OrderHistory, new BrokenHistory { Id = 900, OrderId = 42 }, new OrderHistory { OrderId = 43 }, 42, 42),
                (Indexes.OrderTracking, new BrokenTracking { OrderId = 42 }, new OrderTrackingEntity { OrderId = 43 }, 42, 42),
                (Indexes.LineItems, new BrokenLine { Id = 7, OrderId = 42 }, new SearchResults.LineResult { Id = 8, OrderId = 43 }, 7, 42),
                (Indexes.Comments, new BrokenComment { Id = 7, OrderId = 42 }, new SearchResults.CommentResult { Id = 8, OrderId = 43 }, 7, 42),
                (Indexes.CustomAnswers, new BrokenAnswer { Id = 7, OrderId = 42 }, new SearchResults.CustomFieldResult { Id = 8, OrderId = 43 }, 7, 42),
                (Indexes.Accounts, new BrokenAccount { Id = "A1" }, new Account { Id = "A2" }, "A1", null),
                (Indexes.Buildings, new BrokenBuilding { Id = "B1" }, new Building { Id = "B2" }, "B1", null),
                (Indexes.Commodities, new BrokenCommodity { Id = "C1" }, new Commodity { Id = "C2" }, "C1", null),
                (Indexes.Vendors, new BrokenVendor { Id = "V1" }, new Vendor { Id = "V2" }, "V1", null)
            };
            foreach (var test in cases)
            {
                using var logs = new CapturedLogs();
                var backend = new RecordingBackend("Bonsai");
                Assert.ThrowsExactly<AggregateException>(() => backend.Write(test.Index, new[] { test.Failed, test.Healthy }));
                Assert.AreEqual(1, backend.Documents.Count);
                Assert.AreNotEqual(test.Id.ToString(), backend.Documents.Single().Id);
                var failure = logs.Events.Single(e => Operation(e) == $"Bonsai/{backend.IndexName(test.Index)} serialization");
                CollectionAssert.AreEqual(new[] { test.Id }, DocumentValues(failure, "DocumentId"));
                CollectionAssert.AreEqual(new[] { test.OrderId }, DocumentValues(failure, "OrderId"));
                AssertSafeLogs(logs);
            }
        }

        [DataTestMethod]
        [DataRow("{\"timed_out\":true,\"hits\":{\"hits\":[]}}")]
        [DataRow("{\"_shards\":{\"failed\":1},\"hits\":{\"hits\":[]}}")]
        [DataRow("{\"error\":{\"type\":\"failed\"}}")]
        [DataRow("not JSON")]
        public void IncompleteSearchResponsesAreErrors(string response)
        {
            var backend = new RecordingBackend("Bonsai") { SearchResponse = response };
            var search = new ElasticSearchService(new SearchBackendSet(backend, backend));
            Assert.ThrowsExactly<SearchBackendException>(() => search.SearchOrders("paper", new[] { 42 }));
        }

        [TestMethod]
        public void InheritedOptInSerializationExcludesUnmarkedProperties()
        {
            var backend = new RecordingBackend("Bonsai");
            backend.Write(Indexes.Accounts, new[] { new OptInDocument { Id = 42, Included = "visible", Excluded = "private" } });
            var source = backend.Documents.Single().Source;
            Assert.AreEqual(42, source.Value<int>("id"));
            Assert.AreEqual("visible", source.Value<string>("included"));
            Assert.IsNull(source["excluded"]);
        }

        [TestMethod]
        public void JobOperationsFinishBeforeAggregateFailure()
        {
            var completed = new List<int>();
            Assert.ThrowsExactly<AggregateException>(() => IndexingFailures.RunAll(
                () => { completed.Add(1); throw new InvalidOperationException(); },
                () => completed.Add(2),
                () => { completed.Add(3); throw new InvalidOperationException(); },
                () => completed.Add(4)));
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, completed);
        }

        [TestMethod]
        public void ReplacementFailureDoesNotPromoteOrDeleteActiveIndex()
        {
            var backend = new RecordingBackend("Bonsai") { FailingDocument = "42" };
            Assert.ThrowsExactly<AggregateException>(() => backend.Write(Indexes.OrderTracking,
                new[] { new OrderTrackingEntity { OrderId = 42 } }, recreate: true));
            Assert.IsFalse(backend.Calls.Any(c => c.Method == "DELETE" || c.Path == "/_aliases"));
            Assert.AreEqual("42", backend.Documents.Single().Id);
        }

        [TestMethod]
        public void ReplacementIsRefreshedBeforeAtomicAliasPromotion()
        {
            var backend = new RecordingBackend("Bonsai");
            backend.Write(Indexes.Commodities, new[] { new Commodity { Id = "A", Name = "Paper" } }, recreate: true);
            var promotion = backend.Calls.Single(c => c.Path == "/_aliases");
            var actions = JObject.Parse(promotion.Body)["actions"];
            Assert.AreEqual("test-commodities", actions[0]["remove_index"]["index"]);
            Assert.AreEqual("test-commodities", actions[1]["add"]["alias"]);
            Assert.IsTrue(backend.Calls.FindIndex(c => c.Path.EndsWith("/_refresh")) < backend.Calls.IndexOf(promotion));
        }

        [TestMethod]
        public void DocumentsMatchLegacyNewtonsoftSerializationAndUseStableHistoryIdentity()
        {
            var legacy = new ElasticClient(new ConnectionSettings(new Elasticsearch.Net.SingleNodeConnectionPool(new Uri("http://localhost:9200")), JsonNetSerializer.Default));
            var document = new OrderHistory { Id = 999, OrderId = 42, RequestNumber = "OLD-42", DateCreated = new DateTime(2012, 3, 4), LastActionDate = new DateTime(2026, 10, 26), TotalAmount = 12.34m };
            using var stream = new MemoryStream();
            legacy.SourceSerializer.Serialize(document, stream);
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var expected = JObject.Parse(reader.ReadToEnd());
            expected["id"] = 42;
            var backend = new RecordingBackend("Bonsai");
            backend.Write(Indexes.OrderHistory, new[] { document });
            Assert.IsTrue(JToken.DeepEquals(expected, backend.Documents.Single().Source), $"Expected {expected}; actual {backend.Documents.Single().Source}");
            Assert.AreEqual("42", backend.Documents.Single().Id);
            var mapping = IndexBackend.Mapping<OrderHistory>();
            Assert.AreEqual(1000000, mapping["settings"]["index.max_terms_count"]);
            Assert.AreEqual("date", mapping["mappings"]["properties"]["lastActionDate"]["type"]);
            Assert.AreEqual("keyword", mapping["mappings"]["properties"]["statusId"]["fields"]["keyword"]["type"]);
        }

        [TestMethod]
        public void EmptyPermissionsNeverContactBackendAndReadFailureNeverFallsBack()
        {
            var reader = new RecordingBackend("ElasticCloud") { FailSearch = true };
            var other = new RecordingBackend("Bonsai");
            var set = new SearchBackendSet(reader, other, reader);
            var search = new ElasticSearchService(set);
            Assert.AreEqual(0, search.SearchOrders("test", Array.Empty<int>()).Count);
            Assert.AreEqual(0, search.SearchComments("test", Array.Empty<int>()).Count);
            Assert.AreEqual(0, search.SearchLineItems("test", Array.Empty<int>()).Count);
            Assert.AreEqual(0, search.SearchCustomFieldAnswers("test", Array.Empty<int>()).Count);
            Assert.AreEqual(0, search.GetLatestComments(10, Array.Empty<int>()).Count);
            Assert.AreEqual(0, search.GetOrderHistory(Array.Empty<int>(), null, null, null, null, null).Results.Count);
            Assert.AreEqual(0, reader.Calls.Count);
            Assert.ThrowsExactly<SearchBackendException>(() => search.SearchOrders("test", new[] { 42 }));
            Assert.AreEqual(0, other.Calls.Count);
        }

        [TestMethod]
        public void SearchControllerReportsUnavailableInsteadOfZeroResults()
        {
            var search = new Mock<ISearchService>();
            search.Setup(s => s.SearchOrders("paper", It.IsAny<int[]>())).Throws(new SearchBackendException("Unavailable"));
            var access = new Mock<IAccessQueryService>();
            access.Setup(a => a.GetOrderAccess(It.IsAny<string>())).Returns(Array.Empty<Access>());
            var controller = new SearchController(search.Object, null, Mock.Of<IUserIdentity>(), access.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            var result = (ViewResult)controller.Results("paper");
            Assert.IsTrue(((SearchResultModel)result.Model).IsUnavailable);
            Assert.AreEqual(503, controller.Response.StatusCode);
        }

        internal sealed class FakeOrderSource : IOrderIndexSource
        {
            private readonly int _count;
            public List<int[]> Reads { get; } = new List<int[]>();
            public IEnumerable<SearchResults.CommentResult> Comments { get; set; } = Array.Empty<SearchResults.CommentResult>();
            public FakeOrderSource(int count) { _count = count; }
            public int[] ChangedOrderIds(DateTime since) => Enumerable.Range(1, _count).ToArray();
            public OrderIndexBatch ReadBatch(int[] ids)
            {
                Reads.Add(ids);
                return new OrderIndexBatch
                {
                    History = ids.Select(i => new OrderHistory { OrderId = i }).ToList(),
                    Lines = ids.Select(i => new SearchResults.LineResult { Id = i, OrderId = i }).ToList(),
                    Answers = ids.Select(i => new SearchResults.CustomFieldResult { Id = i, OrderId = i }).ToList(),
                    Tracking = ids.Select(i => new OrderTrackingEntity { OrderId = i }).ToList(),
                    Comments = ids.Select(i => new SearchResults.CommentResult { Id = i, OrderId = i }).ToList()
                };
            }
            public IEnumerable<SearchResults.CommentResult> RecentComments(DateTime since) => Comments;
        }

        internal sealed class RecordingBackend : IndexBackend
        {
            public string FailingDocument { get; set; }
            public Func<string, string, string, (int? Status, string Body)?> ResponseOverride { get; set; }
            public bool FailSearch { get; set; }
            public bool ThrowOnRequest { get; set; }
            public string SearchResponse { get; set; } = "{\"hits\":{\"hits\":[]}}";
            public List<(string Method, string Path, string Body)> Calls { get; } = new();
            public List<(string Index, string Id, JObject Source)> Documents { get; } = new();
            public List<int[]> Deletes { get; } = new();
            public RecordingBackend(string name) : base(name, "test") { }
            protected override (int? Status, string Body) Send(string method, string path, string body)
            {
                Calls.Add((method, path, body));
                if (ThrowOnRequest) throw new TimeoutException("private transport detail");
                var response = ResponseOverride?.Invoke(method, path, body);
                if (response.HasValue) return response.Value;
                if (path.Contains("/_search")) return FailSearch ? (503, "{}") : (200, SearchResponse);
                if (path.Contains("/_delete_by_query"))
                {
                    Deletes.Add(JObject.Parse(body)["query"]["terms"]["orderId"].Values<int>().ToArray());
                    return (200, "{\"failures\":[],\"version_conflicts\":0}");
                }
                if (path.EndsWith("/_alias"))
                {
                    var index = path.Split('/')[1];
                    return (200, new JObject { [index] = new JObject { ["aliases"] = new JObject() } }.ToString());
                }
                if (path == "/_bulk")
                {
                    var lines = body.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    var items = new JArray();
                    for (var i = 0; i < lines.Length; i += 2)
                    {
                        var metadata = JObject.Parse(lines[i])["index"];
                        var id = metadata.Value<string>("_id");
                        Documents.Add((metadata.Value<string>("_index"), id, JObject.Parse(lines[i + 1])));
                        items.Add(new JObject { ["index"] = new JObject { ["_id"] = id, ["status"] = id == FailingDocument ? 400 : 201 } });
                    }
                    return (200, new JObject { ["items"] = items }.ToString());
                }
                return (200, "{\"acknowledged\":true}");
            }
        }

        private static string Operation(LogEvent entry) => entry.Properties.TryGetValue("Operation", out var value)
            ? (string)((ScalarValue)value).Value : null;

        private static int[] OrderIds(LogEvent entry) => ((SequenceValue)entry.Properties["OrderIds"]).Elements
            .Cast<ScalarValue>().Select(v => (int)v.Value).ToArray();

        private static object[] DocumentValues(LogEvent entry, string key) => ((SequenceValue)entry.Properties["Documents"]).Elements
            .Cast<DictionaryValue>().Select(d => ((ScalarValue)d.Elements.Single(p => (string)p.Key.Value == key).Value).Value).ToArray();

        private static void AssertSafeLogs(CapturedLogs logs)
        {
            Assert.IsFalse(logs.Events.Any(e => (e.RenderMessage() + e.Exception).Contains("private")));
        }

        private sealed class CapturedLogs : IDisposable
        {
            private readonly ILogger _previous = Log.Logger;
            private readonly Logger _logger;
            public List<LogEvent> Events { get; } = new();
            public CapturedLogs()
            {
                var sink = new Mock<ILogEventSink>();
                sink.Setup(s => s.Emit(It.IsAny<LogEvent>())).Callback<LogEvent>(Events.Add);
                _logger = new LoggerConfiguration().WriteTo.Sink(sink.Object).CreateLogger();
                Log.Logger = _logger;
            }
            public void Dispose()
            {
                Log.Logger = _previous;
                _logger.Dispose();
            }
        }

        private sealed class BrokenHistory : OrderHistory { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenTracking : OrderTrackingEntity { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenLine : SearchResults.LineResult { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenComment : SearchResults.CommentResult { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenAnswer : SearchResults.CustomFieldResult { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenAccount : Account { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenBuilding : Building { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenCommodity : Commodity { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }
        private sealed class BrokenVendor : Vendor { [JsonProperty] public string Broken => throw new InvalidOperationException("private document body"); }

        private sealed class OptInDocument : UCDArch.Core.DomainModel.DomainObject
        {
            [JsonProperty]
            public string Included { get; set; }
            public string Excluded { get; set; }
        }
    }
}
