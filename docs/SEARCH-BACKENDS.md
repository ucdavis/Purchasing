# How search works

SQL is the source of purchasing data. Background jobs copy selected SQL records into Elasticsearch 9, which serves order searches, history, lookup suggestions, and tracking reports. Search results reflect the last successful indexing work, so they can lag behind SQL.

The web application and indexing jobs use one Elasticsearch endpoint per environment. Both must use the same endpoint and index prefix to read and update the same data.

## Indexed data

Logical index names combine the configured prefix with the lowercase index type. For example, the order-history index in production is `purchasing-prod-orderhistory`.

| Index type | Contents and use |
| --- | --- |
| `OrderHistory` | Order summaries for search, history, and workgroup reports |
| `LineItems` | Searchable line-item descriptions and details |
| `CustomAnswers` | Answers to workgroup custom questions |
| `Comments` | Order comments and recent-comment lists |
| `OrderTracking` | Order status, completion times, and role-based report metrics |
| `Accounts` | Active account lookup suggestions |
| `Buildings` | Building lookup suggestions |
| `Commodities` | Active commodity lookup suggestions |
| `Vendors` | Vendor lookup suggestions |

History and tracking documents use `OrderId` as their Elasticsearch document ID. Child records and lookup records use their own IDs. Stable IDs let subsequent writes replace the same record instead of creating duplicates.

Documents use camel-case JSON properties and the model's Newtonsoft serialization attributes. String fields have searchable text and `.keyword` fields for exact values and aggregations. Dates, numbers, and booleans use their corresponding Elasticsearch types.

## Queries and access control

Order searches query history, line items, custom answers, or comments using Elasticsearch query-string syntax. The application supplies the order IDs the user may access, and the search service filters results to those IDs. An empty permission list returns no results without contacting Elasticsearch. Workgroup reports filter by the supplied workgroups and date range.

Order-history queries support creation dates, last-action dates, and status, sorted by latest action first. Tracking reports calculate completion-time averages and role-based percentiles in Elasticsearch. Account, building, commodity, and vendor lookups return up to ten suggestions.

Search requests time out after 60 seconds. On the search page, malformed query syntax returns HTTP 400 with an invalid-syntax message. Backend failures return HTTP 503 with an unavailable message. Timeouts, shard failures, malformed responses, and unsuccessful requests are treated as errors, not empty result sets.

## Keeping indexes up to date

The order-update job finds orders with tracking records created in the preceding ten minutes. It processes distinct order IDs in batches of at most 1,000, loading their history, line items, custom answers, comments, and tracking data from SQL.

For each successfully loaded set of records, the job:

1. Refreshes the index so documents from previous writes are visible to deletion.
2. Removes the indexed records for the selected orders.
3. Writes their current SQL records using stable document IDs.

An empty SQL result removes stale indexed records. A failed SQL query is handled differently: the job preserves the existing documents for that set of records and records the failure. It continues processing the other indexes and later batches, then reports an aggregate failure if any operation failed.

Comments also have a separate update operation that writes comments created in the preceding ten minutes. The lookup job rebuilds accounts, buildings, commodities, and vendors from SQL.

Updates to related indexes become visible separately. A search can briefly see different stages of an order's update. Reconciliation is not a transaction: a write failure after deletion can leave missing documents until the affected orders are processed successfully again.

### Full rebuilds

A full-index operation writes a new physical index, refreshes it, and switches the logical alias only after the new data has been written successfully. A failure before alias promotion leaves the active index in place. Cleanup can also fail after promotion, when the replacement is already active. Inspect the logical alias before deleting any physical index; never delete an index targeted by an active alias. Unreferenced failed replacements remain available for inspection and manual cleanup.

After a successful switch, older generations in the same namespace are removed only if no other aliases reference them. Rebuilds must run separately from other writers because there is no cross-process coordination to prevent concurrent work from overwriting newer data.

### Freshness and recovery

The ten-minute polling window is not a durable queue. Failed updates can leave that window without being retried, and changes to joined data or deletions may not create a tracking record. A successful job therefore does not prove that every indexed document matches SQL. Repair affected orders or rebuild the relevant indexes when changes have been missed.

The displayed dates are not reliable freshness checkpoints. History shows a five-minute estimate derived from the request time. System index dates use an Elasticsearch indexing-duration field rather than the time of the last successful update; missing values can display a minimum date, and duration strings can fail conversion. Use job outcomes and comparisons with SQL to investigate freshness.

Failure logs include the affected index and available order/document IDs without logging document bodies or credentials. Check extraction, refresh, deletion, bulk-write, and alias errors when diagnosing missing or stale results.

## Configuration

Configure MVC and jobs independently using these settings. In Azure App Service, replace colons with double underscores, for example `Search__ElasticCloud__Url`.

| Setting | Default | Purpose |
| --- | --- | --- |
| `Search:ElasticCloud:Url` | Required | HTTP(S) Elasticsearch endpoint |
| `Search:ElasticCloud:IndexPrefix` | `opp` | Namespace for the nine logical indexes; deployed environments use `purchasing-prod` and `purchasing-test` |
| `Search:ElasticCloud:Username` and `Password` | Unset | Basic authentication; supply both |
| `Search:ElasticCloud:ApiKey` | Unset | Encoded API key, as an alternative to basic authentication |

Credentials embedded in the endpoint URL are also supported. Configure only one credential source and keep it in secret configuration. Invalid endpoints, prefixes, and conflicting credentials fail service initialization without making a network request.

Settings are read at process startup. Restart the affected application after changing them. Search configuration is independent of [OpenTelemetry logging, tracing, and metrics](TELEMETRY.md).

Before using a deployment slot, verify its endpoint, prefix, credentials, and job scheduling flags. Staging can share production SQL and search, so its scheduled jobs must remain disabled. Swapping slots does not copy or validate index data.

### Changing endpoints or prefixes

Reusing the same populated endpoint and prefix preserves the existing data. A matching prefix on another endpoint does not establish completeness. Before directing reads to a new, empty, or incomplete target, backfill orders and all lookup indexes and verify their IDs and contents against SQL. Coordinate indexing writers during backfill and verification. Also pause SQL writes through verification and cutover, or capture and reconcile every SQL change since the backfill snapshot before directing reads to the target. Changes older than ten minutes can escape the polling window while indexing is paused; restarting the scheduled job does not repair them or populate older missing records.

## Code and local testing

- [IndexSearchService.cs](../Purchasing.Core/Services/IndexSearchService.cs) defines searches, history queries, access filters, and report aggregations.
- [IndexService.cs](../Purchasing.Core/Services/IndexService.cs) coordinates indexing operations; [OrderIndexSource.cs](../Purchasing.Core/Services/OrderIndexSource.cs) reads the SQL records.
- [IndexBackend.cs](../Purchasing.Core/Services/IndexBackend.cs) handles mappings, serialization, reconciliation, bulk writes, aliases, and response validation.
- [ElasticCloudIndexBackend.cs](../Purchasing.Core/Services/ElasticCloudIndexBackend.cs) sends requests through the Elasticsearch 9 client; [SearchEndpoint.cs](../Purchasing.Core/Services/SearchEndpoint.cs) validates configuration.

To run the integration tests, set `PURCHASING_TEST_ELASTIC9_URL` to a disposable local Elasticsearch 9 instance and run:

```sh
dotnet test Purchasing.Tests/Purchasing.Tests.csproj --filter FullyQualifiedName~SearchBackendIntegrationTests
```

The tests require a loopback endpoint, create uniquely named indexes, and remove their indexes afterward. They are inconclusive when the endpoint is unset. They do not require production credentials.
