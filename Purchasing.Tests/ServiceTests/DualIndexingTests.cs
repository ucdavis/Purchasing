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

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
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

        [TestMethod]
        public void DeleteFailureDoesNotStopOtherIndexesOrDestination()
        {
            var bonsai = new RecordingBackend("Bonsai") { FailDeletes = true };
            var cloud = new RecordingBackend("ElasticCloud");
            var source = new FakeOrderSource(1);
            var service = new ElasticSearchIndexService(null, new SearchBackendSet(bonsai, bonsai, cloud), source);
            Assert.ThrowsExactly<AggregateException>(() => service.UpdateOrderIndexes());
            Assert.AreEqual(5, bonsai.Deletes.Count);
            Assert.AreEqual(0, bonsai.Documents.Count);
            Assert.AreEqual(5, cloud.Documents.Count);
        }

        [TestMethod]
        public void UnavailableDestinationDoesNotStopHealthyDestination()
        {
            var unavailable = new RecordingBackend("ElasticCloud") { ThrowOnRequest = true };
            var healthy = new RecordingBackend("Bonsai");
            var service = new ElasticSearchIndexService(null, new SearchBackendSet(healthy, unavailable, healthy), new FakeOrderSource(1));
            var exception = Assert.ThrowsExactly<AggregateException>(() => service.UpdateOrderIndexes());
            Assert.AreEqual(5, unavailable.Calls.Count);
            Assert.AreEqual(5, healthy.Documents.Count);
            Assert.IsFalse(exception.ToString().Contains("private transport detail"));
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
            public IEnumerable<SearchResults.CommentResult> RecentComments(DateTime since) => Array.Empty<SearchResults.CommentResult>();
        }

        internal sealed class RecordingBackend : IndexBackend
        {
            public string FailingDocument { get; set; }
            public bool FailDeletes { get; set; }
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
                if (path.Contains("/_search")) return FailSearch ? (503, "{}") : (200, SearchResponse);
                if (path.Contains("/_delete_by_query"))
                {
                    Deletes.Add(JObject.Parse(body)["query"]["terms"]["orderId"].Values<int>().ToArray());
                    return (200, FailDeletes ? "{\"failures\":[{\"reason\":\"failed\"}]}" : "{\"failures\":[],\"version_conflicts\":0}");
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

        private sealed class OptInDocument : UCDArch.Core.DomainModel.DomainObject
        {
            [JsonProperty]
            public string Included { get; set; }
            public string Excluded { get; set; }
        }
    }
}
