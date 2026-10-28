using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Helpers;

namespace Purchasing.Core.Services
{
    public sealed class BackfillOptions
    {
        public BackfillSelection Selection { get; set; } = new();
        public int BatchSize { get; set; } = 100;
        public int MaxBatches { get; set; } = int.MaxValue;
        public TimeSpan? MaxDuration { get; set; }
        public bool WritersSeparated { get; set; }
        public bool MaintenanceConfirmed { get; set; }
        public void Validate()
        {
            Selection.Validate();
            if (BatchSize < 1 || BatchSize > 1000 || MaxBatches < 1 || MaxDuration <= TimeSpan.Zero)
                throw new ArgumentException("Batch size must be 1..1000; execution limits must be positive.");
        }
    }

    public sealed class BackfillResult
    {
        public bool Complete { get; internal set; }
        public bool ReadyForHandoff { get; internal set; }
        public long Failures { get; internal set; }
        public long Missing { get; internal set; }
        public long Stale { get; internal set; }
        public long Unexpected { get; internal set; }
        public long ExpectedDocuments { get; internal set; }
        public long TargetDocuments { get; internal set; }
        public int WorkUnits { get; internal set; }
        public TimeSpan Elapsed { get; internal set; }
        public int ExitCode => Failures > 0 || Missing + Stale + Unexpected > 0 ? 1 : Complete ? 0 : 2;
    }

    // Public entry point for the local executable. Scheduled jobs keep their existing behavior.
    public sealed class SearchBackfillRunner
    {
        internal static readonly Indexes[] LookupIndexes = { Indexes.Accounts, Indexes.Buildings, Indexes.Commodities, Indexes.Vendors };
        internal static readonly Indexes[] OrderIndexes = { Indexes.OrderHistory, Indexes.LineItems, Indexes.CustomAnswers, Indexes.Comments, Indexes.OrderTracking };
        private readonly ISearchBackfillSource _source;
        private readonly ISearchBackfillTarget _target;
        private readonly Action<string> _log;
        public string SourceIdentity => _source.Identity;
        public string TargetIdentity => _target.Identity;
        public string ProjectionIdentity => _source.SchemaIdentity;

        public SearchBackfillRunner(IDbService db, SearchEndpoint endpoint, Action<string> log)
            : this(new SqlSearchBackfillSource(db), new SearchBackfillTarget(new ElasticCloudIndexBackend(endpoint), endpoint), log) { }
        internal SearchBackfillRunner(ISearchBackfillSource source, ISearchBackfillTarget target, Action<string> log)
        { _source = source; _target = target; _log = log ?? (_ => { }); }

        public void Inspect() => _target.InspectIsolation();

        public BackfillResult Backfill(BackfillOptions options, string checkpoint, bool resume, CancellationToken cancellation = default) =>
            Backfill(options, new FileBackfillCheckpointStore(checkpoint), resume, cancellation);

        internal BackfillResult Backfill(BackfillOptions options, IBackfillCheckpointStore store, bool resume, CancellationToken cancellation = default)
        {
            options.Validate();
            if (!options.WritersSeparated) throw new ArgumentException("Confirm the isolated target and separated writers before backfill.");
            var state = new BackfillCheckpoint
            {
                Source = SourceIdentity, Target = TargetIdentity, Schema = ProjectionIdentity,
                Selection = options.Selection.Identity, BatchSize = options.BatchSize
            };
            if (resume)
            {
                if (!store.Exists) throw new BackfillSafetyException("Resume checkpoint does not exist.");
                var loaded = store.Load();
                loaded.Validate(state);
                state = loaded;
            }
            else
            {
                if (store.Exists) throw new BackfillSafetyException("Checkpoint already exists. Resume it or choose a new path.");
                state.HighWatermark = _source.HighWatermark(options.Selection);
                store.Save(state);
            }
            _target.InspectIsolation();
            var run = new Run(options, cancellation, _log);
            InitializeOrders(run);
            foreach (var ids in state.Pending.ToArray())
            {
                if (!run.Take()) break;
                if (WriteOrders(run, ids)) { state.Pending.Remove(ids); store.Save(state); }
            }
            while (!state.OrdersComplete && state.Pending.Count < 100 && run.Take())
            {
                int[] ids = null;
                if (!run.Try("sql-order-selection", () => ids = _source.NextOrders(options.Selection, state.LastProcessedId, state.HighWatermark, options.BatchSize))) break;
                if (ids.Length == 0)
                {
                    state.OrdersComplete = true;
                    store.Save(state);
                    break;
                }
                // Persist pending IDs before touching Elasticsearch. Successful later batches cannot hide a failed batch.
                state.Pending.Add(ids);
                state.LastProcessedId = ids.Last();
                store.Save(state);
                if (WriteOrders(run, ids)) state.Pending.Remove(ids);
                store.Save(state);
            }
            foreach (var index in LookupIndexes)
            {
                var lookup = state.Lookups[index];
                if (lookup.Complete) continue;
                CopyLookup(run, index, lookup, () => store.Save(state));
                if (lookup.Copied && !run.Stopped && Sweep(run, index, deleteUnexpected: true))
                {
                    lookup.Complete = true;
                    store.Save(state);
                }
            }
            run.Result.Complete = state.OrdersComplete && state.Pending.Count == 0 && state.Lookups.Values.All(l => l.Complete) && run.Result.Failures == 0;
            _log($"baseline pending_batches={state.Pending.Count} incomplete_lookups={state.Lookups.Values.Count(l => !l.Complete)} high_watermark={state.HighWatermark} last_processed={state.LastProcessedId}");
            return run.Finish();
        }

        public BackfillResult FinalizeTest(BackfillOptions options, CancellationToken cancellation = default) => FinalPass(options, true, cancellation);
        public BackfillResult VerifyTest(BackfillOptions options, CancellationToken cancellation = default) => FinalPass(options, false, cancellation);

        private BackfillResult FinalPass(BackfillOptions options, bool reconcile, CancellationToken cancellation)
        {
            options.Validate();
            if (!options.Selection.IsFull || !options.MaintenanceConfirmed || !options.WritersSeparated)
                throw new ArgumentException("Final reconciliation/verification requires the full dataset and confirmed TEST maintenance with separated writers.");
            _target.InspectIsolation();
            var run = new Run(options, cancellation, _log);
            var high = _source.HighWatermark(options.Selection);
            if (reconcile)
            {
                InitializeOrders(run);
                VisitOrders(run, high, ids => WriteOrders(run, ids));
                foreach (var index in LookupIndexes) CopyLookup(run, index, new LookupCheckpoint(), () => { });
                foreach (var index in Enum.GetValues<Indexes>()) Sweep(run, index, deleteUnexpected: true);
            }
            // Independent current-SQL verification does not trust checkpoint completion or earlier write responses.
            foreach (var index in Enum.GetValues<Indexes>()) run.Try($"refresh/{index}", () => _target.Refresh(index));
            VisitOrders(run, high, ids =>
            {
                IReadOnlyList<SearchProjection> projections = null;
                if (!run.Try($"verify-sql/orders/{ids.First()}..{ids.Last()}", () => projections = _source.ReadOrders(ids))) return false;
                foreach (var projection in projections) VerifyProjection(run, projection);
                return true;
            });
            foreach (var index in LookupIndexes)
            {
                string after = null;
                while (run.Take())
                {
                    SearchProjection projection = null;
                    if (!run.Try($"verify-sql/{index}", () => projection = _source.ReadLookup(index, after, options.BatchSize))) break;
                    if (!projection.Succeeded) { run.Failure($"verify-sql/{index}", "extraction"); break; }
                    var documents = projection.Documents;
                    if (documents.Count == 0) break;
                    VerifyProjection(run, projection);
                    after = documents.Last().Id;
                }
            }
            foreach (var index in Enum.GetValues<Indexes>()) Sweep(run, index, deleteUnexpected: false);
            if (!run.Stopped && _source.HighWatermark(options.Selection) != high)
                run.Failure("verify/source", "order high watermark changed during maintenance");
            run.Result.Complete = !run.Stopped && run.Result.Failures == 0 && run.Result.Missing + run.Result.Stale + run.Result.Unexpected == 0;
            run.Result.ReadyForHandoff = run.Result.Complete;
            return run.Finish();
        }

        // A read-only extraction sample. No checkpoint, index writes, maintenance, or readiness claim.
        public BackfillResult Measure(BackfillOptions options, CancellationToken cancellation = default)
        {
            options.Validate();
            if (options.MaxBatches == int.MaxValue) throw new ArgumentException("Measure requires an explicit maximum batch count.");
            var run = new Run(options, cancellation, _log);
            VisitOrders(run, _source.HighWatermark(options.Selection), ids =>
            {
                IReadOnlyList<SearchProjection> projections = null;
                if (!run.Try($"measure-sql/orders/{ids.First()}..{ids.Last()}", () => projections = _source.ReadOrders(ids))) return false;
                foreach (var projection in projections)
                    if (!projection.Succeeded) run.Failure($"measure/{projection.Index}", "extraction");
                    else _log($"measure/{projection.Index} documents={projection.Documents.Count}");
                return true;
            });
            run.Result.Complete = !run.Stopped && run.Result.Failures == 0;
            return run.Finish();
        }

        private void InitializeOrders(Run run)
        {
            if (run.Stopped) return;
            run.Try("initialize-order-indexes", () =>
            {
                foreach (var projection in _source.ReadOrders(Array.Empty<int>()))
                    run.Try($"initialize/{projection.Index}", () => _target.Write(projection));
            });
        }

        private bool WriteOrders(Run run, int[] ids)
        {
            var complete = true;
            IReadOnlyList<SearchProjection> projections = null;
            if (!run.Try($"sql/orders/{ids.First()}..{ids.Last()}", () => projections = _source.ReadOrders(ids))) return false;
            foreach (var projection in projections)
                if (!projection.Succeeded) { run.Failure($"sql/{projection.Index}", "extraction"); complete = false; }
                else complete &= run.Try($"write/{projection.Index}/orders/{ids.First()}..{ids.Last()}", () => _target.Write(projection, ids));
            return complete;
        }

        private void VisitOrders(Run run, int high, Func<int[], bool> visit)
        {
            var after = 0;
            while (run.Take())
            {
                int[] ids = null;
                if (!run.Try("sql-order-selection", () => ids = _source.NextOrders(run.Options.Selection, after, high, run.Options.BatchSize))) break;
                if (ids.Length == 0) break;
                visit(ids);
                after = ids.Last();
            }
        }

        private void CopyLookup(Run run, Indexes index, LookupCheckpoint state, Action save)
        {
            while (!state.Copied && run.Take())
            {
                SearchProjection projection = null;
                if (!run.Try($"sql/{index}", () => projection = _source.ReadLookup(index, state.AfterId, run.Options.BatchSize))) break;
                if (!projection.Succeeded) { run.Failure($"sql/{index}", "extraction"); break; }
                if (!run.Try($"write/{index}", () => _target.Write(projection))) break;
                var documents = projection.Documents;
                if (documents.Count == 0) state.Copied = true;
                else state.AfterId = documents.Last().Id;
                save();
            }
        }

        private void VerifyProjection(Run run, SearchProjection projection)
        {
            if (!projection.Succeeded) { run.Failure($"verify/{projection.Index}", "extraction"); return; }
            run.Try($"verify-get/{projection.Index}", () =>
            {
                var documents = projection.Documents;
                if (documents.Select(d => d.Id).Distinct().Count() != documents.Count)
                    throw new InvalidOperationException("Duplicate projected IDs.");
                foreach (var batch in documents.Partition(run.Options.BatchSize))
                {
                    if (run.Stopped) return;
                    var expected = batch.ToArray();
                    var actual = _target.Get(projection.Index, expected.Select(d => d.Id).ToArray()).ToDictionary(d => d.Id, d => d.Source);
                    run.Result.ExpectedDocuments += expected.Length;
                    foreach (var document in expected)
                        if (!actual.TryGetValue(document.Id, out var source)) { run.Result.Missing++; run.Mismatch(projection.Index, document.Id, "missing"); }
                        else if (!SearchBackfillTarget.SameContent(document.Source, source)) { run.Result.Stale++; run.Mismatch(projection.Index, document.Id, "stale"); }
                }
            });
        }

        private bool Sweep(Run run, Indexes index, bool deleteUnexpected)
        {
            if (run.Stopped) return false;
            var before = run.Result.Failures;
            run.Try($"{(deleteUnexpected ? "reconcile" : "verify")}-scan/{index}", () =>
            {
                _target.Refresh(index);
                foreach (var page in _target.Scan(index, run.Options.BatchSize))
                {
                    if (!run.Take()) break;
                    run.Try($"scan-page/{index}", () =>
                    {
                        var projection = _source.ReadExisting(index, page.Select(d => d.Id).ToArray());
                        if (!projection.Succeeded) { run.Failure($"scan-sql/{index}", "extraction"); return; }
                        var expected = projection.Documents.ToDictionary(d => d.Id, d => d.Source);
                        var unexpected = page.Where(d => !expected.ContainsKey(d.Id)).Select(d => d.Id).ToArray();
                        if (deleteUnexpected) _target.Delete(index, unexpected);
                        else
                        {
                            run.Result.TargetDocuments += page.Count;
                            run.Result.Unexpected += unexpected.Length;
                            foreach (var id in unexpected) run.Mismatch(index, id, "unexpected");
                            // Contents are checked from the SQL side. This second pass establishes absence of extra IDs.
                        }
                    });
                }
            });
            return !run.Stopped && run.Result.Failures == before;
        }

        private sealed class Run
        {
            public BackfillOptions Options { get; }
            public BackfillResult Result { get; } = new();
            private readonly CancellationToken _cancellation;
            private readonly Action<string> _log;
            private readonly Stopwatch _elapsed = Stopwatch.StartNew();
            private bool _budgetStopped;
            private int _diagnostics;
            public Run(BackfillOptions options, CancellationToken cancellation, Action<string> log)
            { Options = options; _cancellation = cancellation; _log = log; }
            public bool Stopped => _budgetStopped || _cancellation.IsCancellationRequested || (Options.MaxDuration.HasValue && _elapsed.Elapsed >= Options.MaxDuration.Value);
            public bool Take()
            {
                if (Stopped) return false;
                if (Result.WorkUnits >= Options.MaxBatches) { _budgetStopped = true; return false; }
                Result.WorkUnits++;
                return true;
            }
            public bool Try(string operation, Action action)
            {
                if (Stopped) return false;
                var clock = Stopwatch.StartNew();
                try { action(); return true; }
                catch (Exception exception)
                {
                    var causes = exception is AggregateException aggregate ? aggregate.Flatten().InnerExceptions.AsEnumerable() : new[] { exception };
                    Failure(operation, string.Join("; ", causes.Take(3).Select(c => c is SearchBackendException || c is BackfillSafetyException ? c.Message : c.GetType().Name)));
                    return false;
                }
                finally { _log($"{operation} elapsed_ms={clock.ElapsedMilliseconds}"); }
            }
            public void Failure(string operation, string reason)
            {
                Result.Failures++;
                _log($"FAILED {operation} reason={reason}");
            }
            public void Mismatch(Indexes index, string id, string kind)
            {
                if (_diagnostics++ < 20) _log($"MISMATCH {index} id={id} kind={kind}");
            }
            public BackfillResult Finish()
            {
                Result.Elapsed = _elapsed.Elapsed;
                _log($"complete={Result.Complete} ready={Result.ReadyForHandoff} failures={Result.Failures} missing={Result.Missing} stale={Result.Stale} unexpected={Result.Unexpected} expected_documents={Result.ExpectedDocuments} target_documents={Result.TargetDocuments} work_units={Result.WorkUnits} elapsed_ms={_elapsed.ElapsedMilliseconds}");
                return Result;
            }
        }
    }
}
