# Search backends

Purchasing ships the existing Bonsai/NEST 7.13.2 transport and the Elasticsearch 9 client together. Both use the same document serialization and query definitions. Search reads use one configured backend. Indexing can write to either backend or both.

## Configuration

Set these values independently in the MVC and jobs applications. App Service environment variables use double underscores, for example `Search__WriteTargets=Both`. Both transports use a 60-second request timeout.

| Setting | Default | Meaning |
| --- | --- | --- |
| `Search:ReadBackend` | `Bonsai` | `Bonsai` or `ElasticCloud`; applies to searches, reports, history, lookups, index counts, and index modification stats. |
| `Search:WriteTargets` | `Bonsai` | `Bonsai`, `ElasticCloud`, or `Both`. |
| `Search:Bonsai:Url` | Existing `ElasticSearchUrl` | Bonsai search endpoint. Existing credentials embedded in that URL remain supported. |
| `Search:ElasticCloud:Url` | Required when selected | Elasticsearch 9 endpoint. |
| `Search:<backend>:IndexPrefix` | `opp` | Namespace for the nine logical index names, such as `opp-orderhistory`. |
| `Search:<backend>:Username` and `Password` | Unset | Optional basic authentication. Supply both. |
| `Search:<backend>:ApiKey` | Unset | Optional encoded API key, as an alternative to basic authentication. |

Only enabled read/write backends require configuration. Invalid selections, endpoints, prefixes, and credential combinations fail service initialization. No network request is needed for configuration validation. Credentials belong in the existing secret configuration, not committed JSON. Search settings are separate from `Stackify.ElasticUrl` and logging/APM settings.

Settings are captured at process startup. Restart each affected application after changes. Verify slot-specific settings before a swap. A swap does not copy indexes, establish freshness, or make manual jobs in staging safe. In particular, staging and production may share SQL and search endpoints.

To enable dual writes while continuing to read Bonsai, configure both endpoints and set `Search:WriteTargets=Both`, leaving `Search:ReadBackend=Bonsai`. Later, selecting `ElasticCloud` or returning to `Bonsai` changes the reader after restart. There is no automatic fallback. These examples describe configuration behavior, not authorization or readiness for a production switch.

## Indexing and failures

SQL extraction runs once per batch. Each configured backend receives the same projected documents. If one projection fails, the other projections are still extracted and written, and later batches continue. Failed projections are skipped without deleting existing documents; successful empty projections still remove stale documents. Scheduled order updates process every batch of at most 1,000 order IDs. History and tracking use `OrderId` as their document identity; child and lookup records use their own IDs. History's incidental SQL row-number `Id` is normalized to `OrderId`. Full history extraction uses the same SQL function as incremental extraction to avoid inconsistent latest-action projections.

For each selected order batch, reconciliation removes existing history, lines, answers, tracking, and comments, then writes their current SQL projections. Refreshing before deletion makes repeated reconciliation remove documents from a previous unrefreshed run. Changes to related indexes become visible separately; brief lag and temporary inconsistency are accepted.

Each target's bulk items and delete responses are checked. Refresh or deletion failures are collected while replacement writes are still attempted. A failure does not prevent other targets, indexes, or later batches from being attempted. Extraction and reconciliation failures log affected order IDs; write failures, including index setup and alias promotion, log available document and order IDs with target/index context, without document bodies or transport credentials. Jobs attempt all their indexing operations and then fail if any work failed. AggieEnterprise processing and other business side effects execute once; only the index writes fan out.

The existing full-index operations write a fresh physical index, refresh it, and promote its alias only after successful writes. The first promotion can atomically replace a legacy concrete index. Later promotions remove previous generations in the same namespace after confirming no other aliases use them. Failed builds leave their physical indexes for inspection; operators must review and remove failed generations to reclaim storage. This is not a coordinated migration runner. Run full rebuilds separately from other writers until cross-process coordination is implemented.

Malformed query-string syntax returns HTTP 400 with an invalid-search-syntax message. Other search errors, including mapping/configuration HTTP 400 responses, remain backend failures. Search timeouts, shard failures, and unsuccessful responses throw an error. For backend failures, the search page returns HTTP 503 and shows an unavailable message. It does not turn an outage into zero matches or try another backend. Other callers use their existing error handling.

## Limits and follow-up work

This PR intentionally adds no durable checkpoints, retry queue, SQL tracking tables, recovery files, automatic backlog catch-up, or cross-process locks. A logged failure is not a promise of a later retry. Updates outside the existing ten-minute polling window may require manual repair. Overlapping admin actions, full rebuilds, and scheduled jobs can still apply older data after newer data.

There is no reliable freshness checkpoint. At the user's request, the History and System index pages retain their existing date displays and behavior. History uses the existing five-minute estimate. System converts the selected backend's `index_time` string as before; default stats omit that field, producing `DateTime.MinValue`, and a duration string cannot be parsed as a date. These values do not prove freshness. [Issue #677](https://github.com/ucdavis/Purchasing/issues/677) tracks truthful backend-specific dates, failure/partial-success handling, timezone behavior, and unknown states.

The tracking feed also misses some mutations and deletions, including changes to joined data. A one-day replay would not prove completeness.

The standalone local rebuild/catch-up runner, verification and completeness requirements, read-switch and rollback freshness gates, and the Bonsai retirement observation period remain follow-up work. SQL resizing, production configuration, deployment, and production rebuilding require separate operational decisions.

## Validation

`DualIndexingTests` exercises configuration, batching across 2,001 orders, independent target failures, delete and bulk errors, stable IDs, legacy serialization, failed replacement protection, the preserved date behavior and selected-backend stats, and the search error response. `ElasticsearchCompatibilityTests` uses an HTTP fixture with Bonsai's Elasticsearch 7.10.2 OSS identity and no `X-Elastic-Product` header.

`SearchBackendIntegrationTests` writes disposable, uniquely named indexes on local Elasticsearch instances. Set `PURCHASING_TEST_ELASTIC7_URL` and `PURCHASING_TEST_ELASTIC9_URL` to loopback endpoints to enable it. It rejects non-loopback endpoints and removes its own indexes afterward. Without those settings these tests are explicitly inconclusive.

The integration suite covers all nine index types, searches, history filters and sorting, authorization and empty permissions, more than 65,536 permitted IDs, tracking aggregations and percentiles, replacement aliases, and repeated reconciliation with stale-child removal. The selected server versions are Elasticsearch 7.10.2 OSS and 9.5.4. No test needs production SQL or production search credentials.
