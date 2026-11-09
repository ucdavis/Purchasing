# Elasticsearch search

Purchasing uses Elasticsearch 9 for searches, reports, history, lookups, index counts, and indexing jobs. Web and jobs use one configured endpoint and the same document serialization and query definitions. Requests have a 60-second timeout.

## Configuration

Configure MVC and jobs independently. App Service uses double underscores, for example `Search__ElasticCloud__Url`.

| Setting | Default | Meaning |
| --- | --- | --- |
| `Search:ElasticCloud:Url` | Required | HTTP(S) Elasticsearch endpoint. |
| `Search:ElasticCloud:IndexPrefix` | `opp` | Namespace for the nine logical indexes. Preserve the deployed `purchasing-prod` and `purchasing-test` prefixes. |
| `Search:ElasticCloud:Username` and `Password` | Unset | Optional basic authentication; supply both. Credentials embedded in the endpoint URL are also supported. |
| `Search:ElasticCloud:ApiKey` | Unset | Optional encoded API key, as an alternative to basic authentication. |

Invalid endpoints, prefixes, and conflicting credential sources fail service initialization without a network request. Keep credentials in secret configuration. Search settings are separate from `Stackify.ElasticUrl` and APM settings. The Serilog sink independently uses `Elasticsearch.Net`, pinned to preserve its existing resolved version.

Settings are captured at process startup. Verify each slot's endpoint, prefix, credentials, and job scheduling flags before a swap. Staging may share production SQL and search; keep its scheduled jobs disabled. A swap does not copy indexes or establish freshness.

## Indexing and failures

SQL extraction runs once per batch. Scheduled order updates process all batches of at most 1,000 order IDs. Failed projections preserve existing documents; successful empty projections remove stale documents. Other projections and later batches continue after a failure. Jobs attempt their operations and report aggregate failure afterward. AggieEnterprise business side effects execute once.

History and tracking use `OrderId` as their document identity; child and lookup records use their own IDs. History's incidental SQL row-number `Id` is normalized to `OrderId`. Full and incremental history extraction use the same SQL function. Newtonsoft attributes, including inherited opt-in serialization, define the existing document format.

Reconciliation refreshes before removing old history, lines, answers, tracking, and comments, then writes their current projections. Related indexes become visible separately. Bulk items, refreshes, deletes, and alias operations are checked. Failure logs include available document/order IDs without document bodies or credentials.

Full-index operations build and refresh a new physical index before promoting its alias. Failed builds do not replace the active index. Promotion can replace an existing concrete index; old generations are removed only after checking their aliases. Failed generations remain for operator inspection. Run full rebuilds separately from other writers until cross-process coordination exists.

Malformed query-string syntax returns HTTP 400 with an invalid-syntax message. Other backend failures return HTTP 503 on the search page, rather than zero matches. Timeouts, shard failures, malformed responses, and unsuccessful requests remain errors.

## Deployment and follow-up

Deploy and verify TEST before PROD. Exercise search, history, reports, lookups, an order update, scheduled indexing, AggieEnterprise commodity indexing, and logging. The existing index format and prefixes require no data migration. Remove obsolete endpoint and backend-selection settings only after deploying the single-endpoint code. Audit all slots, deployment variables, developer secrets, and operational scripts. Retiring the old hosted service and its credentials is a separate operational step after confirming no remaining consumers. Rollback builds must support the existing Elasticsearch 9 configuration.

Draft [PR #679](https://github.com/ucdavis/Purchasing/pull/679) must be rebased and adapted before merging: construct `SearchEndpoint(configuration)` without a backend enum, inject a single `IndexBackend` into services, and update tests that used the previous backend collection. Its runner continues to use `Search:ElasticCloud:*` and must retain its explicit source/target safety checks.

There are no durable checkpoints, retry queues, automatic catch-up, or cross-process locks in deployed jobs. Failed updates outside the ten-minute polling window may need manual repair. The tracking feed misses some mutations and deletions. Overlapping rebuilds and scheduled jobs can still apply older data after newer data.

History retains its existing five-minute date estimate. System index dates retain the existing `index_time` conversion, including `DateTime.MinValue` when absent and a conversion error for duration strings. These displays do not establish freshness. [Issue #677](https://github.com/ucdavis/Purchasing/issues/677) owns their correction.

## Validation

`IndexingTests` covers configuration, 2,001-order batching, projection failures, stale-document reconciliation, bulk and lifecycle failures, stable identities, serialization, authorization, date behavior, and search errors. `ElasticsearchCompatibilityTests` exercises the real Elasticsearch 9 transport through MVC's dependency graph, including order-history authorization IDs and status filters.

`SearchBackendIntegrationTests` requires `PURCHASING_TEST_ELASTIC9_URL` pointing to a disposable loopback Elasticsearch 9 server. It uses a unique prefix and removes its indexes. Without the endpoint the tests are explicitly inconclusive. Coverage includes all nine index types, history filtering and sorting, authorization and empty permissions, more than 65,536 permitted IDs, tracking aggregations, replacement aliases, and repeated reconciliation with stale-child removal. No test requires production credentials.
