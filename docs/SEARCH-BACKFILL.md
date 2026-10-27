# Local TEST search backfill

`Purchasing.SearchBackfill` implements [issue #678](https://github.com/ucdavis/Purchasing/issues/678). It reads live TEST SQL and writes one isolated Purchasing Elastic Cloud namespace. It reuses the application's five order projections, four lookup projections, mappings, stable IDs, serialization, bulk checks, and order reconciliation. It is a local console application, outside the deployed WebJob publish pattern.

The runner does not deploy, scale SQL, change settings, pause jobs or edits, or switch search reads. Implementation approval does not authorize a live backfill or maintenance window. Obtain operational approval for those actions after recording the source, destination, measured performance, and maintenance plan. This TEST procedure is not a production cutover policy.

## Publish and configure

Use the repository's .NET 10 SDK. Publish once and retain that output until its checkpoints are retired. Resume binds to the compiled Core module and the definitions of the five SQL projection objects. Rebuilding different code, or changing a projection view/function, rejects old checkpoints.

```sh
dotnet publish Purchasing.SearchBackfill/Purchasing.SearchBackfill.csproj -c Release -o "$HOME/.local/share/purchasing-search-backfill/build"
dotnet "$HOME/.local/share/purchasing-search-backfill/build/Purchasing.SearchBackfill.dll" --help
```

Keep configuration, checkpoints, and logs outside the checkout. Configuration comes from optional `--config /absolute/path/test.json`, then environment variables prefixed with `PURCHASING_BACKFILL_`. Nested keys use `__`. The runner deliberately ignores the deployed `Search:ReadBackend` and `Search:WriteTargets` settings and constructs only an Elastic Cloud writer.

| Key after the environment prefix | Purpose |
| --- | --- |
| `Source__ConnectionString` | Live TEST SQL connection. Use encryption and certificate validation, with a read-only SQL principal. |
| `Source__ExpectedDatabase` | Explicit database name. Both the connection string and opened connection must match it. |
| `Search__ElasticCloud__Url` | Root HTTPS endpoint, without a path, query, or fragment. |
| `Search__ElasticCloud__IndexPrefix` | Explicit isolated Purchasing namespace. No implicit `opp` default for this tool. |
| `Search__ElasticCloud__ApiKey` | Encoded application API key supplied through external secret configuration. |
| `Search__ElasticCloud__Username` and `Password` | Alternative to an API key. Do not configure both credential forms. |

For example, the nonsecret portion of an external JSON file is:

```json
{
  "Source": { "ExpectedDatabase": "<verified TEST database name>" },
  "Search": {
    "ElasticCloud": {
      "Url": "https://<verified deployment endpoint>/",
      "IndexPrefix": "<approved isolated purchasing prefix>"
    }
  }
}
```

Inject the connection string and search credential from the operator's secret store. Do not put them in command arguments, checkpoints, logs, issues, or the repository. SQL access needs the same SELECT permissions as the shared projections and `VIEW DEFINITION` for `dbo.udf_GetOrderHistoryForOrderIds`, `dbo.vLineResults`, `dbo.vCustomFieldResults`, `dbo.vCommentResults`, and `dbo.vOrderTrackingIndex`. Identity inspection also reads the current database's `sys.databases` row.

## Inspect the actual environment

Run `inspect --config /absolute/path/test.json`. It prints the connected SQL server/database/creation identity, sanitized target endpoint/prefix, and projection fingerprint. It checks the nine logical index names and rejects aliases that resolve to multiple physical indexes, another namespace, other aliases, or filtered/routed aliases. It makes no writes. This is not a capacity or permission test.

Before authorizing writes, record these facts from the deployed environment, using `az` for Azure inspection:

1. Verify the subscription, resource group, TEST app/slot, SQL server/database, current SQL tier, and connection references. Check the source identity printed by the tool against that inventory. The explicit database-name check and `--test-source-confirmed` are operator safeguards, not automatic production detection.
2. Inspect current app settings, WebJob enablement, schedules, and recent runs. Include order updates and recent comments, lookup rebuilding, AggieEnterprise commodity rebuilding and SQL mutations, manual create-index jobs, admin rebuild actions, and any other process that can reach the selected target. Repository schedules alone do not prove deployed behavior.
3. Keep deployed indexing on Bonsai during baseline copying. Confirm that nothing except this runner can write the selected Elastic Cloud destination. Do not enable `Both` during the backfill. Run overlapping selections sequentially; there is no distributed lock or fencing. Separate checkpoint files do not make concurrent same-target runners safe.
4. Inspect the chosen Elastic deployment's current topology, free storage, shard health, indexing pressure, rejections, mappings, and other tenants. Verify that its application key is limited to the nine Purchasing index names and any intended generations. It needs index creation/mapping access, reads and metadata access, indexing, deletion by query, and refresh. Check actual privileges with the deployment administrator. Test permitted operations in an approved disposable Purchasing namespace before bulk use. No existing deployment is reserved by this document.

Existing destination mappings must match the shared application's contract. The runner creates missing indexes with that contract; it does not migrate an incompatible existing mapping. Other applications' indexes must remain outside the selected prefix and key permissions.

## Measure before choosing capacity

Start with current live TEST SQL capacity. `measure` reads bounded order batches and reports SQL extraction elapsed time and projected document counts without touching Elasticsearch or a checkpoint:

```sh
dotnet /path/to/build/Purchasing.SearchBackfill.dll measure --config /path/to/test.json --test-source-confirmed --year 2020 --batch-size 100 --max-batches 5
```

Repeat representative older, newer, and child-heavy selections. A work-unit limit stops this sample with exit code 2. The limit includes selection attempts; it is not an exact requested document count. Retain timestamps, batch sizes, order boundaries, projected counts, and elapsed times. The log reports payload-free operation names and timings; treat IDs and resource names as operational records.

After separate authorization for bounded target writes, use a new checkpoint and a small `backfill` range with `--max-batches` and `--max-minutes`. Compare the `sql/orders`, `write/<index>`, lookup, and scan timings. Collect SQL CPU/DTU, data/log IO, waits/blocking and query durations over the same interval, plus Elasticsearch indexing latency, rejections, heap and disk pressure. Measure network latency from the operator's machine to both services. A high client duration alone does not establish SQL CPU pressure.

Use these measurements to propose a SQL tier and duration. Propose P4 or higher only if SQL saturation/query evidence supports it; slow writes or network traffic need a different remedy. Include storage growth and retry rates in the destination-capacity proposal. Record the proposed change and cost window for approval, including the return to the original tier.

Measure a complete reconciliation plus verification during an approved rehearsal maintenance window before scheduling the handoff window. A partial range estimate is not a full-pass duration. The final pass rereads SQL and scans all nine targets, so its time may exceed baseline copying time. Include recovery time for a restarted final pass. No live capacity or full-pass timing measurements are supplied by the automated tests.

## Baseline, ranges, and resume

```sh
dotnet /path/to/build/Purchasing.SearchBackfill.dll backfill --config /path/to/test.json --test-source-confirmed --writers-separated --checkpoint /path/to/checkpoints/full.json --batch-size 100 --max-minutes 480
dotnet /path/to/build/Purchasing.SearchBackfill.dll backfill --config /path/to/test.json --test-source-confirmed --writers-separated --checkpoint /path/to/checkpoints/full.json --batch-size 100 --max-minutes 480 --resume
```

Omit dates for the complete order dataset. `--year 2020` means `2020-01-01 <= Orders.DateCreated < 2021-01-01`. Alternatively use `--from YYYY-MM-DD` and/or `--until YYYY-MM-DD`. Boundaries are timezone-free Pacific wall-clock values, matching the application's stored `Orders.DateCreated` convention. They are not converted using the operator machine's timezone. The lower bound is inclusive and upper bound exclusive, including across daylight-saving changes.

The tool selects ascending order IDs up to a captured maximum ID. Each selected order brings its complete current child projections, including comments and actions from later years. History eligibility comes from the shared SQL function, whose joins can exclude orders. A raw Orders count is not an expected history count.

Every new run rereads its selected orders. Use a new checkpoint path for each deliberate overlapping range. Per-order reconciliation replaces only selected orders; it does not recreate a full index or discard previously loaded years. Each range also copies all four independent lookup indexes and removes lookup documents no longer in the successful SQL projection. These lookups have no order-year selection.

The default batch size is 100, with a maximum of 1,000 order IDs or lookup documents per SQL page. Child extraction retains one order batch at a time; memory still depends on how many children and how much text those orders contain. Shared bulk writes contain at most 1,000 documents; verification gets and target scan pages use the configured batch size. Target identity scans read only the corresponding SQL document IDs. Nothing enumerates the entire target into an in-memory set.

`--max-batches N` counts order, lookup and scan work units. `--max-minutes N` and Ctrl-C stop at operation boundaries. In-flight SQL/HTTP requests may finish or time out first. The existing order projection query timeout is 30 seconds; selection/lookup queries use 300 seconds, HTTP requests use 60 seconds, and scan contexts expire after five minutes. Lower the order batch size for heavy projections, using a new checkpoint when changing batch size.

The atomic JSON checkpoint binds the source, target, selection, batch size and projection identity. It stores a bounded baseline cursor, pending order-ID batches, and lookup cursors/completion. It contains no credentials, connection strings, or document payloads. A batch becomes complete only after all five extractions and writes succeed. Pending work is saved before writes, so an interrupted write/checkpoint transition safely replays. Later successes do not erase earlier failures. The runner stops advancing new order work at 100 pending batches to bound manifest size, while still attempting independent lookup work.

Resume retries pending batches, skips confirmed baseline batches, and continues lookups from their last successfully written page. A lookup's cleanup scan restarts if interrupted. A tiny work limit that cannot finish that scan will need to be increased. Resume never treats existing target documents as proof of batch completion.

Exit code 0 means this command completed; baseline completion does not mean freshness or handoff readiness. Exit code 1 means failed extraction, writes, verification, or configuration. Exit code 2 means a bounded or interrupted invocation. Failure logs identify the stage/index and order boundaries, while the manifest retains exact pending IDs. Transport details are limited to sanitized backend errors; SQL exceptions are reported by type without row data.

## Final TEST reconciliation and verification

Obtain approval for the measured maintenance window. Pause TEST data edits and all relevant background SQL mutations and target writers. Inspect deployed settings and actual process/job state again, record who established the pause and when, and wait for in-flight work to finish. A flag to this executable does not pause anything or verify Azure settings.

Final commands require full selection, even if baseline copying used years. This allows deleted orders and stale children to be removed without guessing a deleted order's creation year from surviving target documents. Keep the destination exclusively Purchasing and quiescent through verification and handoff.

```sh
dotnet /path/to/build/Purchasing.SearchBackfill.dll finalize --config /path/to/test.json --test-source-confirmed --writers-separated --maintenance-confirmed --batch-size 100
```

`finalize` captures a fresh maximum order ID and reconciles every current order and lookup. It then scans the nine targets in bounded pages and removes identities absent from successful current SQL projections. Extraction failures preserve target documents and remain failures. Finally, a separate current-SQL pass compares every expected ID and normalized serialized document with Elasticsearch, and target scans detect unexpected IDs. Comparison preserves nulls, array order, decimal precision and date-looking text. It normalizes JSON object ordering and equivalent numeric representations. Refreshes, partial search responses, failed bulk items, deletion conflicts, missing documents, stale contents, and extra identities all prevent readiness.

Final passes do not read or skip from baseline checkpoints. They deliberately accept no `--checkpoint` or `--resume`. If interrupted or unsuccessful, keep TEST maintenance controlled, correct the reported problem, and rerun the whole final command. A successful early batch is no exemption from the next final pass. Changing the order high watermark during the pass also blocks readiness; stable IDs alone do not prove other data stayed unchanged, so the operational pause remains essential.

Use `verify` with the same confirmation flags for a read-only complete check after a successful reconciliation. It performs SQL extraction, target reads/refreshes, and scroll cleanup, with no document writes/deletes or index creation. Missing indexes fail verification. See the Elasticsearch [scroll lifecycle](https://www.elastic.co/docs/api/doc/elasticsearch/operation/operation-scroll) and [per-document multi-get failures](https://www.elastic.co/docs/api/doc/elasticsearch/operation/operation-mget) for the response checks used here.

Only a complete final reconciliation/verification or complete verify command can print `READY for manual TEST handoff`. Review its exit code, zero failure/mismatch totals, full-dataset selection, source/target identities, and retained logs. Baseline checkpoints and the existing UI date displays are not freshness evidence. Timestamp display accuracy remains [issue #677](https://github.com/ucdavis/Purchasing/issues/677).

## Manual handoff and recovery

Keep the maintenance pause in place after readiness. Have the operator apply the separately approved Elastic Cloud write configuration to all relevant deployed writers, restart affected processes, and verify the effective settings and target prefix. Complete any independently selected read-routing change. Confirm that obsolete or duplicate writers cannot target the same destination, then resume normal TEST operation and inspect the first scheduled runs. The ordinary ten-minute tracking lookback cannot repair all baseline-era changes and is not a substitute for this final pass.

If final verification fails, retain maintenance while repairing and rerunning, or explicitly abandon that handoff and restore the previous operating state. Do not enable Elastic Cloud scheduled writes midway through a rerunnable final pass. If edits or background mutations resume, the old verification is no longer a handoff gate; establish a new maintenance window and run a fresh full final pass.

For a corrupted checkpoint, restore an intact copy or start a new checkpoint and deliberately reread the selection. Do not edit cursors to bypass failures. For identity mismatch, recover the original settings/published build or start a new run. Correct credentials, permissions, mappings, capacity, or query failures before retrying. Failed or bounded commands never switch the application automatically.

## Focused verification

```sh
dotnet test Purchasing.Tests/Purchasing.Tests.csproj --filter 'FullyQualifiedName~SearchBackfillTests|FullyQualifiedName~DualIndexingTests|FullyQualifiedName~ElasticsearchCompatibilityTests'
```

The runner suite uses mutable SQL-projection fixtures and a stateful Elasticsearch protocol fixture. It exercises interrupted checkpoint transitions, partial writes, earlier failures, lookup resume, identity rejection, overlapping ranges, independent failures, empty projections, post-baseline changes/deletions, all-nine-index verification, bounded child writes/gets/scans, alias isolation, incomplete responses, decimal/date-text comparison, and CLI gates. SQL adapter fixtures exercise bounded lookup cursors and targeted child identity reads. Existing focused tests cover shared indexing behavior and Bonsai transport compatibility.

These tests do not establish live SQL throughput, a real Elastic deployment's capacity/permissions, deployed writer separation, or maintenance readiness. No live backfill, SQL scaling, deployment, job disabling, or read-routing change is part of this implementation validation. The full legacy SQLite-dependent suite is outside this focused macOS run.
