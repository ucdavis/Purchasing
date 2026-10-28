using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Purchasing.Core.Services;

namespace Purchasing.SearchBackfill
{
    internal static class Program
    {
        private const string Help = """
            Purchasing TEST search backfill, .NET 10
            Commands: inspect | measure | backfill | finalize | verify
            --config PATH                 External JSON configuration; environment overrides it.
            --checkpoint PATH             Required for backfill. Existing files require --resume.
            --resume                      Resume the same baseline. New paths deliberately reread SQL.
            --year YYYY                   Order creation year in Pacific wall-clock time.
            --from YYYY-MM-DD              Inclusive creation date. Cannot combine with --year.
            --until YYYY-MM-DD             Exclusive creation date. Cannot combine with --year.
            --batch-size N                1..1000 orders/documents per page, default 100.
            --max-batches N               Work-unit limit, including lookup and scan pages.
            --max-minutes N               Stop between work units after this duration.
            --test-source-confirmed       Confirm this connection is the intended live TEST source.
            --writers-separated           Confirm isolated Purchasing target and no competing writers.
            --maintenance-confirmed       Confirm TEST SQL mutations and indexing are paused.

            finalize and verify require full selection and both writer/maintenance confirmations.
            Final passes always restart from current SQL; they do not use baseline checkpoints.
            inspect and measure are read-only. measure requires --max-batches.
            Environment prefix: PURCHASING_BACKFILL_, nested keys use __.
            Required keys: Source:ConnectionString, Source:ExpectedDatabase,
            Search:ElasticCloud:Url, Search:ElasticCloud:IndexPrefix, plus external credentials.
            Exit codes: 0 complete; 1 failures/mismatches/invalid configuration; 2 bounded/interrupted.
            See docs/SEARCH-BACKFILL.md before any live execution.
            """;

        public static int Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("--help")) { Console.WriteLine(Help); return 0; }
            var stage = "arguments";
            try
            {
                var parsed = Parse(args);
                stage = "configuration";
                var builder = new ConfigurationBuilder();
                if (parsed.Config != null) builder.AddJsonFile(Path.GetFullPath(parsed.Config), optional: false);
                var config = builder.AddEnvironmentVariables("PURCHASING_BACKFILL_").Build();
                if (string.IsNullOrWhiteSpace(config["Source:ExpectedDatabase"]) || string.IsNullOrWhiteSpace(config["Search:ElasticCloud:IndexPrefix"]))
                    throw new UsageException("Set Source:ExpectedDatabase and an explicit isolated Search:ElasticCloud:IndexPrefix.");
                var db = new ConfiguredDatabase(config["Source:ConnectionString"], config["Source:ExpectedDatabase"]);
                var endpoint = new SearchEndpoint(config, SearchBackend.ElasticCloud);
                if (endpoint.Url.Scheme != "https") throw new UsageException("Use HTTPS for the Elastic Cloud endpoint.");
                stage = "source identity and SQL projection definitions";
                var runner = new SearchBackfillRunner(db, endpoint, Console.WriteLine);
                Console.WriteLine($"source={runner.SourceIdentity}\ntarget={runner.TargetIdentity}\nprojection={runner.ProjectionIdentity}");
                using var cancellation = new CancellationTokenSource();
                ConsoleCancelEventHandler handler = (_, e) => { e.Cancel = true; cancellation.Cancel(); };
                Console.CancelKeyPress += handler;
                try
                {
                    stage = parsed.Command;
                    if (parsed.Command == "inspect") { runner.Inspect(); Console.WriteLine("Isolation checks passed. Capacity, privileges, deployed writers, and maintenance still need operator inspection."); return 0; }
                    var result = parsed.Command switch
                    {
                        "measure" => runner.Measure(parsed.Options, cancellation.Token),
                        "backfill" => runner.Backfill(parsed.Options, parsed.Checkpoint, parsed.Resume, cancellation.Token),
                        "finalize" => runner.FinalizeTest(parsed.Options, cancellation.Token),
                        "verify" => runner.VerifyTest(parsed.Options, cancellation.Token),
                        _ => throw new UsageException("Unknown command.")
                    };
                    if (result.ReadyForHandoff) Console.WriteLine("READY for manual TEST handoff. Keep maintenance in place until configuration handoff and writer checks finish.");
                    else Console.WriteLine("No handoff readiness claimed. Review completion, failures, and limits above.");
                    return result.ExitCode;
                }
                finally { Console.CancelKeyPress -= handler; }
            }
            catch (UsageException exception) { Console.Error.WriteLine(exception.Message); return 1; }
            catch (BackfillSafetyException exception) { Console.Error.WriteLine(exception.Message); return 1; }
            catch (SearchBackendException exception) { Console.Error.WriteLine(exception.Message); return 1; }
            catch (Exception exception)
            {
                // SQL, configuration and transport exception text can contain credentials or row data.
                Console.Error.WriteLine($"Failed during {stage} ({exception.GetType().Name}). Check configuration and the last operation reported; no readiness claimed.");
                return 1;
            }
        }

        internal sealed class Arguments
        {
            public string Command { get; set; }
            public string Config { get; set; }
            public string Checkpoint { get; set; }
            public bool Resume { get; set; }
            public BackfillOptions Options { get; } = new();
        }

        internal static Arguments Parse(string[] args)
        {
            var result = new Arguments { Command = args[0] };
            if (!new[] { "inspect", "measure", "backfill", "finalize", "verify" }.Contains(result.Command)) throw new UsageException("Unknown command. Use --help.");
            var seen = new HashSet<string>();
            int? year = null;
            bool test = false;
            for (var i = 1; i < args.Length; i++)
            {
                var option = args[i];
                if (!seen.Add(option)) throw new UsageException("Duplicate option. Use --help.");
                string Value()
                {
                    if (++i >= args.Length || args[i].StartsWith("--", StringComparison.Ordinal)) throw new UsageException("Missing option value.");
                    return args[i];
                }
                switch (option)
                {
                    case "--config": result.Config = Value(); break;
                    case "--checkpoint": result.Checkpoint = Value(); break;
                    case "--resume": result.Resume = true; break;
                    case "--year": year = int.Parse(Value(), CultureInfo.InvariantCulture); break;
                    case "--from": result.Options.Selection.From = Date(Value()); break;
                    case "--until": result.Options.Selection.Until = Date(Value()); break;
                    case "--batch-size": result.Options.BatchSize = int.Parse(Value(), CultureInfo.InvariantCulture); break;
                    case "--max-batches": result.Options.MaxBatches = int.Parse(Value(), CultureInfo.InvariantCulture); break;
                    case "--max-minutes": result.Options.MaxDuration = TimeSpan.FromMinutes(double.Parse(Value(), CultureInfo.InvariantCulture)); break;
                    case "--test-source-confirmed": test = true; break;
                    case "--writers-separated": result.Options.WritersSeparated = true; break;
                    case "--maintenance-confirmed": result.Options.MaintenanceConfirmed = true; break;
                    default: throw new UsageException("Unknown option. Use --help.");
                }
            }
            if (year.HasValue)
            {
                if (!result.Options.Selection.IsFull || year < 1753 || year > 9998) throw new UsageException("Use --year 1753..9998 or --from/--until, not both.");
                result.Options.Selection.From = new DateTime(year.Value, 1, 1);
                result.Options.Selection.Until = new DateTime(year.Value + 1, 1, 1);
            }
            result.Options.Validate();
            if (result.Command != "inspect" && !test) throw new UsageException("Confirm the TEST source with --test-source-confirmed.");
            if (result.Command == "backfill" && (string.IsNullOrWhiteSpace(result.Checkpoint) || !result.Options.WritersSeparated))
                throw new UsageException("Backfill requires --checkpoint and --writers-separated.");
            if (result.Command != "backfill" && (result.Checkpoint != null || result.Resume)) throw new UsageException("Only backfill accepts a checkpoint or --resume.");
            if ((result.Command == "finalize" || result.Command == "verify") && (!result.Options.Selection.IsFull || !result.Options.MaintenanceConfirmed || !result.Options.WritersSeparated))
                throw new UsageException("Final passes require the full dataset, --writers-separated and --maintenance-confirmed.");
            if (result.Command == "measure" && result.Options.MaxBatches == int.MaxValue) throw new UsageException("Measure requires --max-batches.");
            return result;
        }

        private static DateTime Date(string value) => DateTime.SpecifyKind(DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture), DateTimeKind.Unspecified);
        private sealed class UsageException : Exception { public UsageException(string message) : base(message) { } }

        private sealed class ConfiguredDatabase : IDbService
        {
            private readonly string _connectionString;
            private readonly string _expectedDatabase;
            public ConfiguredDatabase(string connectionString, string expectedDatabase)
            {
                var parsed = new SqlConnectionStringBuilder(connectionString);
                if (!string.Equals(parsed.InitialCatalog, expectedDatabase, StringComparison.OrdinalIgnoreCase))
                    throw new UsageException("Source:ExpectedDatabase does not match the connection database.");
                _connectionString = parsed.ConnectionString;
                _expectedDatabase = expectedDatabase;
            }
            public DbConnection GetConnection(string connectionString = null)
            {
                var connection = new SqlConnection(_connectionString);
                try
                {
                    connection.Open();
                    if (!string.Equals(connection.Database, _expectedDatabase, StringComparison.OrdinalIgnoreCase)) throw new UsageException("Connected database differs from the expected TEST database.");
                    return connection;
                }
                catch { connection.Dispose(); throw; }
            }
        }
    }
}
