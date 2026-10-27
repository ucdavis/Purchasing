using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Core.Services;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class SearchBackfillTests
    {
        private static BackfillOptions Options(int size = 2) => new() { BatchSize = size, WritersSeparated = true, MaintenanceConfirmed = true };

        [TestMethod]
        public void EarlierExtractionFailureSurvivesLaterSuccessAndResumeSkipsCompletedBatches()
        {
            var fixture = new Fixture(5);
            fixture.Source.FailProjection = (index, ids) => index == Indexes.LineItems && ids.Contains(1);
            var failed = fixture.Runner.Backfill(Options(), fixture.Store, false);
            Assert.AreEqual(1, failed.ExitCode);
            Assert.AreEqual(5, fixture.Store.State.LastProcessedId);
            CollectionAssert.AreEqual(new[] { 1, 2 }, fixture.Store.State.Pending.Single());
            Assert.IsTrue(fixture.Backend.Data[Indexes.Comments].ContainsKey("5"));
            Assert.IsTrue(fixture.Store.State.Lookups.Values.All(l => l.Complete));
            fixture.Source.FailProjection = null;
            fixture.Source.OrderReads.Clear();
            var resumed = fixture.Runner.Backfill(Options(), fixture.Store, true);
            Assert.AreEqual(0, resumed.ExitCode);
            Assert.IsFalse(resumed.ReadyForHandoff);
            CollectionAssert.AreEqual(new[] { 1, 2 }, fixture.Source.OrderReads.Single(ids => ids.Length > 0));
        }

        [TestMethod]
        public void CrashAfterSuccessfulWritesBeforeCheckpointReplaysThePendingBatch()
        {
            var fixture = new Fixture(2);
            fixture.Store.FailSave = 3;
            Assert.ThrowsExactly<IOException>(() => fixture.Runner.Backfill(Options(), fixture.Store, false));
            CollectionAssert.AreEqual(new[] { 1, 2 }, fixture.Store.State.Pending.Single());
            Assert.AreEqual(2, fixture.Backend.Data[Indexes.OrderHistory].Count);
            fixture.Store.FailSave = 0;
            Assert.AreEqual(0, fixture.Runner.Backfill(Options(), fixture.Store, true).ExitCode);
            Assert.AreEqual(2, fixture.Backend.Data[Indexes.OrderHistory].Count);
            Assert.AreEqual(2, fixture.Source.OrderReads.Count(ids => ids.SequenceEqual(new[] { 1, 2 })));
        }

        [TestMethod]
        public void CancellationBetweenProjectionsLeavesBatchPending()
        {
            var fixture = new Fixture(2);
            using var cancellation = new CancellationTokenSource();
            fixture.Backend.AfterBulk = index => { if (index == Indexes.OrderHistory) cancellation.Cancel(); };
            var interrupted = fixture.Runner.Backfill(Options(), fixture.Store, false, cancellation.Token);
            Assert.AreEqual(2, interrupted.ExitCode);
            Assert.AreEqual(1, fixture.Store.State.Pending.Count);
            Assert.AreEqual(0, fixture.Backend.Data[Indexes.LineItems].Count);
            fixture.Backend.AfterBulk = null;
            Assert.AreEqual(0, fixture.Runner.Backfill(Options(), fixture.Store, true).ExitCode);
            Assert.AreEqual(2, fixture.Backend.Data[Indexes.LineItems].Count);
        }

        [TestMethod]
        public void BulkItemFailureKeepsLookupCursorAndOtherLookupsContinue()
        {
            var fixture = new Fixture(1);
            fixture.Source.AddLookup(Indexes.Accounts, "B");
            fixture.Backend.FailWrite = (index, id) => index == Indexes.Accounts && id == "B";
            Assert.AreEqual(1, fixture.Runner.Backfill(Options(1), fixture.Store, false).ExitCode);
            Assert.AreEqual("A", fixture.Store.State.Lookups[Indexes.Accounts].AfterId);
            Assert.IsFalse(fixture.Store.State.Lookups[Indexes.Accounts].Complete);
            Assert.IsTrue(fixture.Store.State.Lookups[Indexes.Vendors].Complete);
            fixture.Backend.FailWrite = null;
            Assert.AreEqual(0, fixture.Runner.Backfill(Options(1), fixture.Store, true).ExitCode);
            Assert.AreEqual(2, fixture.Backend.Data[Indexes.Accounts].Count);
        }

        [DataTestMethod]
        [DataRow("source")]
        [DataRow("target")]
        [DataRow("schema")]
        [DataRow("selection")]
        [DataRow("size")]
        public void ResumeRejectsIdentityChangesBeforeAnyTargetWrite(string field)
        {
            var fixture = new Fixture(1);
            fixture.Runner.Backfill(Options(), fixture.Store, false);
            var state = fixture.Store.State;
            switch (field)
            {
                case "source": state.Source += "different"; break;
                case "target": state.Target += "different"; break;
                case "schema": state.Schema += "different"; break;
                case "selection": state.Selection += "different"; break;
                case "size": state.BatchSize++; break;
            }
            fixture.Store.Save(state);
            fixture.Backend.Mutations = 0;
            Assert.ThrowsExactly<BackfillSafetyException>(() => fixture.Runner.Backfill(Options(), fixture.Store, true));
            Assert.AreEqual(0, fixture.Backend.Mutations);
        }

        [TestMethod]
        public void AtomicFileCheckpointContainsOnlyIdentitiesAndProgressAndRejectsCorruption()
        {
            var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                var path = Path.Combine(directory, "run.json");
                var fixture = new Fixture(1);
                Assert.AreEqual(0, fixture.Runner.Backfill(Options(), path, false).ExitCode);
                var text = File.ReadAllText(path);
                foreach (var forbidden in new[] { "api-secret", "credential-user", "private-document", "ConnectionString", "Password", "ApiKey" })
                    Assert.IsFalse(text.Contains(forbidden), forbidden);
                var store = new FileBackfillCheckpointStore(path);
                var loaded = store.Load();
                loaded.Validate(loaded);
                Assert.AreEqual(1, Directory.GetFiles(directory).Length);
                Assert.ThrowsExactly<BackfillSafetyException>(() => fixture.Runner.Backfill(Options(), path, false));
                var missingField = JObject.Parse(text);
                missingField.Remove("Pending");
                File.WriteAllText(path, missingField.ToString());
                Assert.ThrowsExactly<JsonSerializationException>(() => store.Load());
                File.WriteAllText(path, "{broken");
                Assert.ThrowsExactly<JsonReaderException>(() => store.Load());
            }
            finally { Directory.Delete(directory, true); }
        }

        [TestMethod]
        public void SequentialOverlappingRangesReconcileChildrenWithoutRemovingOtherYears()
        {
            var fixture = new Fixture(3);
            fixture.Source.Orders[1] = new DateTime(2020, 1, 1);
            fixture.Source.Orders[2] = new DateTime(2021, 1, 1);
            fixture.Source.Orders[3] = new DateTime(2022, 1, 1);
            var first = Options();
            first.Selection = new() { From = new DateTime(2020, 1, 1), Until = new DateTime(2022, 1, 1) };
            Assert.AreEqual(0, fixture.Runner.Backfill(first, fixture.Store, false).ExitCode);
            Assert.IsTrue(fixture.Backend.Data[Indexes.Comments].ContainsKey("1"), "A comment created years later belongs to its selected order.");
            fixture.Source.Documents[Indexes.LineItems].Remove("2");
            var second = Options();
            second.Selection = new() { From = new DateTime(2021, 1, 1), Until = new DateTime(2023, 1, 1) };
            Assert.AreEqual(0, fixture.Runner.Backfill(second, new MemoryStore(), false).ExitCode);
            CollectionAssert.AreEquivalent(new[] { "1", "2", "3" }, fixture.Backend.Data[Indexes.OrderHistory].Keys.ToArray());
            CollectionAssert.AreEquivalent(new[] { "1", "3" }, fixture.Backend.Data[Indexes.LineItems].Keys.ToArray());
        }

        [DataTestMethod]
        [DataRow(Indexes.OrderHistory)]
        [DataRow(Indexes.LineItems)]
        [DataRow(Indexes.CustomAnswers)]
        [DataRow(Indexes.Comments)]
        [DataRow(Indexes.OrderTracking)]
        public void FailedProjectionPreservesTargetWhileSuccessfulEmptyRemovesIt(Indexes index)
        {
            var fixture = new Fixture(1);
            fixture.Runner.Backfill(Options(), fixture.Store, false);
            fixture.Source.Documents[index].Clear();
            fixture.Source.FailProjection = (i, ids) => i == index && ids.Length > 0;
            var failed = new MemoryStore();
            Assert.AreEqual(1, fixture.Runner.Backfill(Options(), failed, false).ExitCode);
            Assert.AreEqual(1, fixture.Backend.Data[index].Count);
            fixture.Source.FailProjection = null;
            Assert.AreEqual(0, fixture.Runner.Backfill(Options(), failed, true).ExitCode);
            Assert.AreEqual(0, fixture.Backend.Data[index].Count);
        }

        [TestMethod]
        public void FinalPassRepairsPostBaselineUpdatesInsertsDeletesAndAllLookups()
        {
            var fixture = new Fixture(3);
            Assert.AreEqual(0, fixture.Runner.Backfill(Options(), fixture.Store, false).ExitCode);
            ((SearchResults.LineResult)fixture.Source.Documents[Indexes.LineItems]["1"]).PaidNotes = "updated without tracking";
            fixture.Source.DeleteOrder(2);
            fixture.Source.AddOrder(4);
            fixture.Source.Documents[Indexes.Comments].Remove("3");
            fixture.Source.Documents[Indexes.OrderHistory].Remove("3"); // SQL joins can legitimately exclude this order.
            foreach (var index in SearchBackfillRunner.LookupIndexes)
            {
                fixture.Source.Documents[index].Clear();
                fixture.Source.AddLookup(index, "B");
            }
            fixture.Backend.Data[Indexes.LineItems]["orphan"] = new JObject { ["orderId"] = 999, ["id"] = "orphan" };
            var final = fixture.Runner.FinalizeTest(Options());
            Assert.AreEqual(0, final.ExitCode);
            Assert.IsTrue(final.ReadyForHandoff);
            CollectionAssert.AreEquivalent(new[] { "1", "4" }, fixture.Backend.Data[Indexes.OrderHistory].Keys.ToArray());
            Assert.AreEqual("updated without tracking", fixture.Backend.Data[Indexes.LineItems]["1"].Value<string>("paidNotes"));
            Assert.IsFalse(fixture.Backend.Data[Indexes.LineItems].ContainsKey("orphan"));
            Assert.IsFalse(fixture.Backend.Data.Values.Any(d => d.ContainsKey("2")));
            foreach (var index in SearchBackfillRunner.LookupIndexes) CollectionAssert.AreEqual(new[] { "B" }, fixture.Backend.Data[index].Keys.ToArray());
            Assert.IsTrue(final.ExpectedDocuments > 0);
            Assert.AreEqual(final.ExpectedDocuments, final.TargetDocuments);
            Assert.AreEqual(0, fixture.Backend.Scrolls.Count);
        }

        [TestMethod]
        public void VerifyFindsMissingStaleAndUnexpectedDespiteEqualCountsAndDoesNotWrite()
        {
            var fixture = new Fixture(3);
            fixture.Runner.FinalizeTest(Options());
            fixture.Backend.Data[Indexes.LineItems]["1"]["quantity"] = 123;
            fixture.Backend.Data[Indexes.Comments].Remove("2");
            fixture.Backend.Data[Indexes.Comments]["extra"] = new JObject { ["id"] = "extra", ["orderId"] = 3 };
            fixture.Backend.Mutations = 0;
            var verification = fixture.Runner.VerifyTest(Options());
            Assert.AreEqual(1, verification.ExitCode);
            Assert.IsFalse(verification.ReadyForHandoff);
            Assert.AreEqual(1L, verification.Missing);
            Assert.AreEqual(1L, verification.Stale);
            Assert.AreEqual(1L, verification.Unexpected);
            Assert.AreEqual(0, fixture.Backend.Mutations);
        }

        [DataTestMethod]
        [DataRow(Indexes.Accounts)]
        [DataRow(Indexes.Buildings)]
        [DataRow(Indexes.Commodities)]
        [DataRow(Indexes.Vendors)]
        public void LookupReadFailurePreservesDocumentsAndFinalCannotAnnounceReadiness(Indexes index)
        {
            var fixture = new Fixture(1);
            fixture.Runner.FinalizeTest(Options());
            fixture.Source.Documents[index].Clear();
            fixture.Source.FailLookup = index;
            var failed = fixture.Runner.FinalizeTest(Options());
            Assert.AreEqual(1, failed.ExitCode);
            Assert.IsFalse(failed.ReadyForHandoff);
            Assert.AreEqual(1, fixture.Backend.Data[index].Count);
            Assert.IsFalse(fixture.Log.Any(l => l.Contains("private SQL password")));
            fixture.Source.FailLookup = null;
            Assert.IsTrue(fixture.Runner.FinalizeTest(Options()).ReadyForHandoff);
            Assert.AreEqual(0, fixture.Backend.Data[index].Count);
        }

        [TestMethod]
        public void BoundedBaselineResumesWithinOriginalWatermarkThenFinalIncludesNewOrders()
        {
            var fixture = new Fixture(5);
            var limited = Options();
            limited.MaxBatches = 1;
            var first = fixture.Runner.Backfill(limited, fixture.Store, false);
            Assert.AreEqual(2, first.ExitCode);
            Assert.AreEqual(2, fixture.Store.State.LastProcessedId);
            fixture.Source.AddOrder(6);
            Assert.AreEqual(0, fixture.Runner.Backfill(Options(), fixture.Store, true).ExitCode);
            Assert.IsFalse(fixture.Backend.Data[Indexes.OrderHistory].ContainsKey("6"));
            Assert.IsTrue(fixture.Source.OrderReads.All(ids => ids.Length <= 2));
            Assert.IsTrue(fixture.Runner.FinalizeTest(Options()).ReadyForHandoff);
            Assert.IsTrue(fixture.Backend.Data[Indexes.OrderHistory].ContainsKey("6"));
            Assert.IsTrue(fixture.Backend.LargestScanPage <= 2);
        }

        [TestMethod]
        public void BoundedFinalRestartsEveryOrderAndNeverClaimsReadinessEarly()
        {
            var fixture = new Fixture(5);
            var limited = Options();
            limited.MaxBatches = 1;
            var partial = fixture.Runner.FinalizeTest(limited);
            Assert.AreEqual(2, partial.ExitCode);
            Assert.IsFalse(partial.ReadyForHandoff);
            fixture.Source.OrderReads.Clear();
            Assert.IsTrue(fixture.Runner.FinalizeTest(Options()).ReadyForHandoff);
            Assert.IsTrue(fixture.Source.OrderReads.Any(ids => ids.SequenceEqual(new[] { 1, 2 })));
        }

        [TestMethod]
        public void EmptyDatasetCreatesAllNineIndexesAndVerifiesZeroDocuments()
        {
            var fixture = new Fixture(0);
            foreach (var lookup in SearchBackfillRunner.LookupIndexes) fixture.Source.Documents[lookup].Clear();
            var result = fixture.Runner.FinalizeTest(Options());
            Assert.IsTrue(result.ReadyForHandoff);
            Assert.AreEqual(0L, result.ExpectedDocuments);
            Assert.AreEqual(9, fixture.Backend.Existing.Count);
        }

        [TestMethod]
        public void FinalPassRequiresFullSelectionAndMaintenanceBeforeAnyMutation()
        {
            var fixture = new Fixture(1);
            var options = Options();
            options.Selection.From = new DateTime(2020, 1, 1);
            Assert.ThrowsExactly<ArgumentException>(() => fixture.Runner.FinalizeTest(options));
            options.Selection.From = null;
            options.MaintenanceConfirmed = false;
            Assert.ThrowsExactly<ArgumentException>(() => fixture.Runner.FinalizeTest(options));
            Assert.AreEqual(0, fixture.Backend.Mutations);
        }

        [TestMethod]
        public void SourceChangingDuringVerificationPreventsReadiness()
        {
            var fixture = new Fixture(1);
            fixture.Source.AfterHighWatermark = count => { if (count == 2) fixture.Source.AddOrder(2); };
            var result = fixture.Runner.FinalizeTest(Options());
            Assert.IsFalse(result.ReadyForHandoff);
            Assert.AreEqual(1, result.ExitCode);
        }

        [TestMethod]
        public void SerializationUsesStableHistoryIdentityAndNormalizesNumbersWithoutHidingFieldChanges()
        {
            var history = new OrderHistory { OrderId = 42 };
            var doc = IndexBackend.SerializeDocument(history);
            Assert.AreEqual("42", doc.Id);
            Assert.AreEqual(42, doc.Source.Value<int>("id"));
            Assert.IsTrue(SearchBackfillTarget.SameContent(JObject.Parse("{\"n\":1,\"a\":[1.0,null]}"), JObject.Parse("{\"a\":[1,null],\"n\":1.0}")));
            Assert.IsFalse(SearchBackfillTarget.SameContent(JObject.Parse("{\"n\":1}"), JObject.Parse("{\"n\":1,\"extra\":null}")));
            Assert.IsFalse(SearchBackfillTarget.SameContent(JObject.Parse("{\"a\":[1,2]}"), JObject.Parse("{\"a\":[2,1]}")));
        }

        [TestMethod]
        public void VerificationPreservesDateLookingTextAndDecimalPrecision()
        {
            var fixture = new Fixture(1);
            ((SearchResults.CommentResult)fixture.Source.Documents[Indexes.Comments]["1"]).Text = "2020-01-01T00:00:00.000Z";
            ((SearchResults.LineResult)fixture.Source.Documents[Indexes.LineItems]["1"]).Quantity = 0.1234567890123456789012345678m;
            Assert.IsTrue(fixture.Runner.FinalizeTest(Options()).ReadyForHandoff);
        }

        [TestMethod]
        public void ChildBulkAndVerificationPagesStayBoundedForLargeOrderProjections()
        {
            var fixture = new Fixture(1);
            for (var id = 2; id <= 2001; id++)
                fixture.Source.Documents[Indexes.Comments][id.ToString()] = new SearchResults.CommentResult { Id = id, OrderId = 1, Text = "comment" };
            Assert.IsTrue(fixture.Runner.FinalizeTest(Options(100)).ReadyForHandoff);
            Assert.AreEqual(2001, fixture.Backend.Data[Indexes.Comments].Count);
            Assert.IsTrue(fixture.Backend.LargestBulk <= 1000);
            Assert.IsTrue(fixture.Backend.LargestGet <= 100);
            Assert.IsTrue(fixture.Backend.LargestScanPage <= 100);
        }

        [TestMethod]
        public void ScanFailureOnOnePageDoesNotHideLaterIndependentPagesOrOtherIndexes()
        {
            var fixture = new Fixture(3);
            fixture.Runner.FinalizeTest(Options(1));
            fixture.Backend.Data[Indexes.LineItems]["101"] = new JObject { ["orderId"] = 999 };
            fixture.Backend.Data[Indexes.LineItems]["103"] = new JObject { ["orderId"] = 999 };
            fixture.Source.FailProjection = (index, ids) => index == Indexes.LineItems && ids.Contains(101);
            var result = fixture.Runner.FinalizeTest(Options(1));
            Assert.AreEqual(1, result.ExitCode);
            Assert.IsTrue(fixture.Backend.Data[Indexes.LineItems].ContainsKey("101"));
            Assert.IsFalse(fixture.Backend.Data[Indexes.LineItems].ContainsKey("103"));
            Assert.AreEqual(0, fixture.Backend.Scrolls.Count);
        }

        [DataTestMethod]
        [DataRow("outside")]
        [DataRow("shared")]
        [DataRow("filtered")]
        [DataRow("multiple")]
        public void AliasIsolationFailureStopsBeforeWrites(string kind)
        {
            var fixture = new Fixture(1);
            fixture.Backend.Override = (method, path, body) =>
            {
                if (!path.EndsWith("/_alias")) return null;
                var alias = path.Split('/')[1];
                var physical = kind == "outside" ? "other-app-index" : alias + "-generation";
                var aliases = new JObject { [alias] = kind == "filtered" ? new JObject { ["filter"] = new JObject() } : new JObject() };
                if (kind == "shared") aliases["other-app"] = new JObject();
                var response = new JObject { [physical] = new JObject { ["aliases"] = aliases } };
                if (kind == "multiple") response[alias + "-second"] = new JObject { ["aliases"] = aliases.DeepClone() };
                return (200, response.ToString());
            };
            Assert.ThrowsExactly<BackfillSafetyException>(() => fixture.Runner.FinalizeTest(Options()));
            Assert.AreEqual(0, fixture.Backend.Mutations);
        }

        [DataTestMethod]
        [DataRow("/_mget", "{\"docs\":[]}")]
        [DataRow("/_mget", "{\"docs\":[{\"_id\":\"1\",\"error\":{\"reason\":\"private response\"}}]}")]
        [DataRow("/_search?", "{\"_scroll_id\":\"s\",\"_shards\":{\"failed\":1},\"hits\":{\"hits\":[]}}")]
        [DataRow("/_search?", "{\"hits\":{\"hits\":[]}}")]
        [DataRow("/_delete_by_query", "{\"failures\":[],\"version_conflicts\":1,\"deleted\":0}")]
        public void PartialOrFailedBackendResponsesCannotProduceReadiness(string pathPart, string response)
        {
            var fixture = new Fixture(1);
            fixture.Runner.FinalizeTest(Options(1));
            fixture.Backend.Data[Indexes.Accounts]["extra"] = new JObject { ["id"] = "extra" };
            fixture.Backend.Override = (_, path, _) => path.Contains(pathPart) ? (200, response) : null;
            var result = fixture.Runner.FinalizeTest(Options(1));
            Assert.IsFalse(result.ReadyForHandoff);
            Assert.AreEqual(1, result.ExitCode);
            Assert.IsFalse(fixture.Log.Any(l => l.Contains("private response")));
        }

        [TestMethod]
        public void CliParsesPacificYearAndRejectsAmbiguousOrUnsafeOperationsWithoutConnections()
        {
            var args = Purchasing.SearchBackfill.Program.Parse(new[] { "backfill", "--year", "2020", "--checkpoint", "test.json", "--test-source-confirmed", "--writers-separated" });
            Assert.AreEqual(new DateTime(2020, 1, 1), args.Options.Selection.From);
            Assert.AreEqual(new DateTime(2021, 1, 1), args.Options.Selection.Until);
            Assert.AreEqual(DateTimeKind.Unspecified, args.Options.Selection.From.Value.Kind);
            Assert.Throws<Exception>(() => Purchasing.SearchBackfill.Program.Parse(new[] { "finalize", "--test-source-confirmed" }));
            Assert.Throws<Exception>(() => Purchasing.SearchBackfill.Program.Parse(new[] { "backfill", "--year", "2020", "--from", "2020-01-01" }));
            Assert.Throws<Exception>(() => Purchasing.SearchBackfill.Program.Parse(new[] { "verify", "--checkpoint", "test.json" }));
            Assert.Throws<Exception>(() => Purchasing.SearchBackfill.Program.Parse(new[] { "inspect", "--batch-size", "1001" }));
            Assert.Throws<Exception>(() => Purchasing.SearchBackfill.Program.Parse(new[] { "measure", "--test-source-confirmed" }));
        }

        [TestMethod]
        public void MeasureReadsBoundedSqlWithoutTargetRequestsOrCheckpoint()
        {
            var fixture = new Fixture(10);
            var options = Options(2);
            options.MaxBatches = 2;
            var result = fixture.Runner.Measure(options);
            Assert.AreEqual(2, result.ExitCode);
            Assert.IsFalse(result.ReadyForHandoff);
            Assert.AreEqual(2, fixture.Source.OrderReads.Count);
            Assert.AreEqual(0, fixture.Backend.Requests);
            Assert.IsFalse(fixture.Store.Exists);
        }

        [TestMethod]
        public void SharedSqlChildIdentityReadLoadsOnlyRequestedProjectionAndUsesChildIds()
        {
            var reads = 0;
            var db = DualIndexingTests.Database(command =>
            {
                reads++;
                Assert.IsTrue(command.CommandText.Contains("WHERE Id IN"));
                Assert.IsTrue(command.Parameters.Cast<DbParameter>().Any(p => Equals(p.Value, 900)));
                var rows = new DataTable();
                rows.Columns.Add("Id", typeof(int));
                rows.Columns.Add("OrderId", typeof(int));
                rows.Columns.Add("PaidNotes", typeof(string));
                rows.Rows.Add(900, 42, "current paid note");
                return rows.CreateDataReader();
            });
            var projection = new SqlOrderIndexSource(db.Object).ReadDocuments(Indexes.LineItems, new[] { 900 });
            Assert.AreEqual(1, reads);
            Assert.AreEqual(42, projection.Lines.Single().OrderId);
            Assert.AreEqual(900, projection.Lines.Single().Id);
            Assert.AreEqual("current paid note", projection.Lines.Single().PaidNotes);
            Assert.IsNull(projection.History);
            Assert.IsNull(projection.Comments);
        }

        [DataTestMethod]
        [DataRow(Indexes.Accounts)]
        [DataRow(Indexes.Buildings)]
        [DataRow(Indexes.Commodities)]
        [DataRow(Indexes.Vendors)]
        public void LookupSqlPageUsesBoundedCursorAndSharedDomainSerialization(Indexes index)
        {
            var db = DualIndexingTests.Database(command =>
            {
                var parameters = command.Parameters.Cast<DbParameter>().ToArray();
                Assert.IsTrue(parameters.Any(p => p.ParameterName == "size" && Equals(p.Value, 1)));
                Assert.IsTrue(parameters.Any(p => p.ParameterName == "after" && Equals(p.Value, "A")));
                var rows = new DataTable();
                rows.Columns.Add("Id", typeof(string));
                rows.Columns.Add(index == Indexes.Buildings ? "BuildingName" : "Name", typeof(string));
                rows.Rows.Add("B", "second row");
                return rows.CreateDataReader();
            });
            var projection = LookupIndexSource.Read(db.Object, index, "A", 1);
            Assert.IsTrue(projection.Succeeded);
            Assert.AreEqual("B", projection.Documents.Single().Id);
            Assert.AreEqual("second row", projection.Documents.Single().Source.Value<string>(index == Indexes.Buildings ? "buildingName" : "name"));
        }

        private sealed class Fixture
        {
            public FakeSource Source { get; } = new();
            public MemoryBackend Backend { get; } = new();
            public MemoryStore Store { get; } = new();
            public List<string> Log { get; } = new();
            public SearchBackfillRunner Runner { get; }
            public Fixture(int orders)
            {
                for (var id = 1; id <= orders; id++) Source.AddOrder(id);
                foreach (var index in SearchBackfillRunner.LookupIndexes) Source.AddLookup(index, "A");
                var endpoint = new SearchEndpoint(DualIndexingTests.Config(("Search:ElasticCloud:Url", "https://credential-user:api-secret@localhost:9200"),
                    ("Search:ElasticCloud:IndexPrefix", "runner-test")), SearchBackend.ElasticCloud);
                Runner = new SearchBackfillRunner(Source, new SearchBackfillTarget(Backend, endpoint), Log.Add);
            }
        }

        private sealed class MemoryStore : IBackfillCheckpointStore
        {
            private string _json;
            private int _saves;
            public int FailSave { get; set; }
            public bool Exists => _json != null;
            public BackfillCheckpoint State => Load();
            public BackfillCheckpoint Load() => JsonConvert.DeserializeObject<BackfillCheckpoint>(_json);
            public void Save(BackfillCheckpoint checkpoint)
            {
                if (++_saves == FailSave) throw new IOException("simulated interrupted checkpoint");
                _json = JsonConvert.SerializeObject(checkpoint);
            }
        }

        private sealed class FakeSource : ISearchBackfillSource
        {
            public string Identity => "sql-test/purchasing-test/2026-01-01";
            public string SchemaIdentity => "projection-v1";
            public SortedDictionary<int, DateTime> Orders { get; } = new();
            public Dictionary<Indexes, Dictionary<string, object>> Documents { get; } = Enum.GetValues<Indexes>().ToDictionary(i => i, _ => new Dictionary<string, object>());
            public List<int[]> OrderReads { get; } = new();
            public Func<Indexes, int[], bool> FailProjection { get; set; }
            public Indexes? FailLookup { get; set; }
            public Action<int> AfterHighWatermark { get; set; }
            private int _highReads;
            public void AddOrder(int id)
            {
                Orders[id] = new DateTime(2020, 1, 1);
                Documents[Indexes.OrderHistory][id.ToString()] = new OrderHistory { OrderId = id, Justification = "private-document" };
                Documents[Indexes.LineItems][id.ToString()] = new SearchResults.LineResult { Id = id, OrderId = id, Quantity = 2 };
                Documents[Indexes.CustomAnswers][id.ToString()] = new SearchResults.CustomFieldResult { Id = id, OrderId = id, Answer = "answer" };
                Documents[Indexes.Comments][id.ToString()] = new SearchResults.CommentResult { Id = id, OrderId = id, DateCreated = new DateTime(2026, 1, 1), Text = "later comment" };
                Documents[Indexes.OrderTracking][id.ToString()] = new OrderTrackingEntity { OrderId = id };
            }
            public void DeleteOrder(int id)
            {
                Orders.Remove(id);
                foreach (var index in SearchBackfillRunner.OrderIndexes)
                    foreach (var doc in Documents[index].ToArray())
                        if (IndexBackend.SerializeDocument(doc.Value).Source.Value<int>("orderId") == id) Documents[index].Remove(doc.Key);
            }
            public void AddLookup(Indexes index, string id)
            {
                object value = index switch
                {
                    Indexes.Accounts => new Account { Name = "Account" },
                    Indexes.Buildings => new Building { BuildingName = "Building" },
                    Indexes.Commodities => new Commodity { Name = "Commodity" },
                    Indexes.Vendors => new Vendor { Name = "Vendor" },
                    _ => throw new InvalidOperationException()
                };
                value.GetType().GetProperty("Id").SetValue(value, id);
                Documents[index][id] = value;
            }
            private IEnumerable<int> Selected(BackfillSelection selection) => Orders.Where(o =>
                (!selection.From.HasValue || o.Value >= selection.From) && (!selection.Until.HasValue || o.Value < selection.Until)).Select(o => o.Key);
            public int HighWatermark(BackfillSelection selection)
            {
                AfterHighWatermark?.Invoke(++_highReads);
                return Selected(selection).DefaultIfEmpty().Max();
            }
            public int[] NextOrders(BackfillSelection selection, int after, int through, int size) => Selected(selection).Where(id => id > after && id <= through).Take(size).ToArray();
            public IReadOnlyList<SearchProjection> ReadOrders(int[] ids)
            {
                OrderReads.Add(ids);
                return SearchBackfillRunner.OrderIndexes.Select(index => Projection(index,
                    FailProjection?.Invoke(index, ids) == true ? null : Documents[index].Values.Where(d => ids.Contains(IndexBackend.SerializeDocument(d).Source.Value<int>("orderId"))).ToArray())).ToArray();
            }
            public SearchProjection ReadLookup(Indexes index, string after, int size, string[] ids = null)
            {
                if (FailLookup == index) throw new InvalidOperationException("private SQL password");
                return Projection(index, Documents[index].OrderBy(d => d.Key, StringComparer.Ordinal).Where(d => ids != null ? ids.Contains(d.Key) : after == null || StringComparer.Ordinal.Compare(d.Key, after) > 0)
                    .Take(size).Select(d => d.Value).ToArray());
            }
            public SearchProjection ReadExisting(Indexes index, string[] ids)
            {
                if (SearchBackfillRunner.LookupIndexes.Contains(index)) return ReadLookup(index, null, ids.Length, ids);
                var numericIds = ids.Select(id => int.TryParse(id, out var number) ? number : 0).ToArray();
                return Projection(index, FailProjection?.Invoke(index, numericIds) == true ? null :
                    Documents[index].Where(d => ids.Contains(d.Key)).Select(d => d.Value).ToArray());
            }
            private static SearchProjection Projection(Indexes index, object[] docs) => index switch
            {
                Indexes.OrderHistory => new SearchProjection<OrderHistory>(index, docs?.Cast<OrderHistory>().ToArray()),
                Indexes.LineItems => new SearchProjection<SearchResults.LineResult>(index, docs?.Cast<SearchResults.LineResult>().ToArray()),
                Indexes.Comments => new SearchProjection<SearchResults.CommentResult>(index, docs?.Cast<SearchResults.CommentResult>().ToArray()),
                Indexes.CustomAnswers => new SearchProjection<SearchResults.CustomFieldResult>(index, docs?.Cast<SearchResults.CustomFieldResult>().ToArray()),
                Indexes.OrderTracking => new SearchProjection<OrderTrackingEntity>(index, docs?.Cast<OrderTrackingEntity>().ToArray()),
                Indexes.Accounts => new SearchProjection<Account>(index, docs?.Cast<Account>().ToArray()),
                Indexes.Buildings => new SearchProjection<Building>(index, docs?.Cast<Building>().ToArray()),
                Indexes.Commodities => new SearchProjection<Commodity>(index, docs?.Cast<Commodity>().ToArray()),
                Indexes.Vendors => new SearchProjection<Vendor>(index, docs?.Cast<Vendor>().ToArray()),
                _ => throw new InvalidOperationException()
            };
        }

        // Exercises the real shared serializer, bulk validation, replacement, mget and scan protocol.
        private sealed class MemoryBackend : IndexBackend
        {
            public Dictionary<Indexes, Dictionary<string, JObject>> Data { get; } = Enum.GetValues<Indexes>().ToDictionary(i => i, _ => new Dictionary<string, JObject>());
            public HashSet<Indexes> Existing { get; } = new();
            public Dictionary<string, Queue<JObject[]>> Scrolls { get; } = new();
            public int Mutations { get; set; }
            public int Requests { get; private set; }
            public int LargestScanPage { get; private set; }
            public int LargestBulk { get; private set; }
            public int LargestGet { get; private set; }
            public Func<Indexes, string, bool> FailWrite { get; set; }
            public Action<Indexes> AfterBulk { get; set; }
            public Func<string, string, string, (int?, string)?> Override { get; set; }
            public MemoryBackend() : base("ElasticCloud", "runner-test") { }
            protected override (int? Status, string Body) Send(string method, string path, string body)
            {
                Requests++;
                var response = Override?.Invoke(method, path, body);
                if (response.HasValue) return response.Value;
                var name = path.Split('/')[1];
                var index = Enum.GetValues<Indexes>().FirstOrDefault(i => IndexName(i) == name);
                var request = body != null && path != "/_bulk" ? JObject.Parse(body) : null;
                if (path == "/_bulk")
                {
                    Mutations++;
                    var lines = body.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    LargestBulk = Math.Max(LargestBulk, lines.Length / 2);
                    var items = new JArray();
                    for (var i = 0; i < lines.Length; i += 2)
                    {
                        var metadata = JObject.Parse(lines[i])["index"];
                        index = Enum.GetValues<Indexes>().Single(idx => IndexName(idx) == metadata.Value<string>("_index"));
                        var id = metadata.Value<string>("_id");
                        var failed = FailWrite?.Invoke(index, id) == true;
                        if (!failed) Data[index][id] = JsonConvert.DeserializeObject<JObject>(lines[i + 1], new JsonSerializerSettings
                        {
                            DateParseHandling = DateParseHandling.None, FloatParseHandling = FloatParseHandling.Decimal
                        });
                        items.Add(new JObject { ["index"] = new JObject { ["_id"] = id, ["status"] = failed ? 429 : 201 } });
                    }
                    AfterBulk?.Invoke(index);
                    return (200, new JObject { ["items"] = items }.ToString());
                }
                if (path == "/_search/scroll")
                {
                    if (method == "DELETE")
                    {
                        foreach (var id in request["scroll_id"].Values<string>()) Scrolls.Remove(id);
                        return (200, "{\"succeeded\":true}");
                    }
                    return Page(request.Value<string>("scroll_id"));
                }
                if (path.Contains("/_search?"))
                {
                    if (!Existing.Contains(index)) return (404, "{}");
                    var size = request.Value<int>("size");
                    var hits = Data[index].Select(d => new JObject { ["_id"] = d.Key, ["_source"] = d.Value.DeepClone() }).ToArray();
                    var id = Guid.NewGuid().ToString();
                    Scrolls[id] = new Queue<JObject[]>(hits.Chunk(size));
                    return Page(id);
                }
                if (path.EndsWith("/_mget"))
                {
                    LargestGet = Math.Max(LargestGet, request["ids"].Count());
                    var docs = new JArray(request["ids"].Values<string>().Select(id => Data[index].TryGetValue(id, out var source)
                        ? new JObject { ["_id"] = id, ["found"] = true, ["_source"] = source.DeepClone() }
                        : new JObject { ["_id"] = id, ["found"] = false }));
                    return (200, new JObject { ["docs"] = docs }.ToString());
                }
                if (path.EndsWith("/_alias")) return Existing.Contains(index)
                    ? (200, new JObject { [name] = new JObject { ["aliases"] = new JObject() } }.ToString()) : (404, "{}");
                if (path.Contains("/_delete_by_query"))
                {
                    Mutations++;
                    var query = request["query"];
                    var keys = query["ids"] != null ? query["ids"]["values"].Values<string>().Where(id => Data[index].ContainsKey(id)).ToArray()
                        : Data[index].Where(d => query["terms"]["orderId"].Values<int>().Contains(d.Value.Value<int>("orderId"))).Select(d => d.Key).ToArray();
                    foreach (var key in keys) Data[index].Remove(key);
                    return (200, new JObject { ["failures"] = new JArray(), ["version_conflicts"] = 0, ["deleted"] = keys.Length }.ToString());
                }
                if (method == "HEAD") return (Existing.Contains(index) ? 200 : 404, "{}");
                if (method == "PUT") { Mutations++; Existing.Add(index); return (200, "{\"acknowledged\":true}"); }
                if (path.EndsWith("/_refresh")) return Existing.Contains(index) ? (200, "{\"_shards\":{\"failed\":0}}") : (404, "{}");
                throw new InvalidOperationException($"Unexpected request {method} {path}");
            }
            private (int?, string) Page(string id)
            {
                var hits = Scrolls[id].Count > 0 ? Scrolls[id].Dequeue() : Array.Empty<JObject>();
                LargestScanPage = Math.Max(LargestScanPage, hits.Length);
                return (200, new JObject { ["_scroll_id"] = id, ["hits"] = new JObject { ["hits"] = new JArray(hits) } }.ToString());
            }
        }
    }
}
