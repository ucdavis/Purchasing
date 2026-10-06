# .NET 10 upgrade and manual testing plan

This upgrade removes AutoMapper, then updates the web app, libraries, tests and six WebJobs to .NET 10. Five jobs have schedules; CreateOrderIndexes is manual. The final acceptance pass covers the combined change. The application keeps MVC, NHibernate, its existing SQL schema and Windows App Service hosting.

## AutoMapper replacement

Explicit mapping was chosen for this small footprint to avoid the newer AutoMapper licensing requirements. Seven methods replace the eight active calls: six in [EditableValues](../Purchasing.Core/Helpers/EditableValues.cs) and [SearchResults.OrderResult.FromHistory](../Purchasing.Core/Queries/SearchResults.cs). AutoMapper's package, profiles, container registration and injected `IMapper` parameters are removed. Test fixtures no longer initialize AutoMapper.

| ID | Original call site | Replacement and behavior | Manual check |
| --- | --- | --- | --- |
| AM1 | `Purchasing.Core/Services/IndexSearchService.cs`, `SearchOrders` | `OrderResult.FromHistory` copies all 14 result fields. The result ID is `OrderId`, and delivery fields come from `ShipTo` and `ShipToEmail`. Search queries and indexes are unchanged. | Search for an existing order. Compare recipient, requester, dates, purpose, reference, PO, tag, approver, account manager and purchaser. Open the result and confirm it is the correct order. |
| AM2 | `Purchasing.Mvc/Services/WorkgroupService.cs`, `TransferValues` | `CopyVendor` preserves the destination ID and workgroup. Existing KFS and Aggie Enterprise lookups still override supplier/address values afterward. | Create and edit a manual vendor, then an AE supplier/site, through both Workgroup and Wizard flows. Check cleared optional fields, supplier data and unchanged ownership. Exercise the legacy KFS path if still used. |
| AM3 | `Purchasing.Mvc/Services/WorkgroupService.cs`, `CreateWorkgroup` | `CopyWorkgroup` copies settings into a fresh workgroup. The workflow resolves selected organizations and includes the primary organization. | Create a workgroup with primary and additional organizations. Confirm its settings, new ID, organizations and expected inherited permissions. Ensure no accounts, addresses, vendors or orders were cloned from another workgroup. |
| AM4 | `Purchasing.Mvc/Controllers/WorkgroupController.cs`, POST `Edit` | `CopyWorkgroup` updates settings on the loaded entity. Organization selection remains explicit in the controller. | Edit settings on a populated workgroup. Verify accounts, addresses, vendors, orders and direct permissions survive. Change organizations and administrative/inheritance flags, and check that the existing permission rules still apply. Submit invalid data and verify the form and selections redisplay correctly. |
| AM5 | `Purchasing.Mvc/Controllers/WorkgroupController.cs`, POST `EditAddress` | `CopyAddress` copies address data to a fresh record, preserving the destination ID and workgroup. Existing duplicate matching/reactivation and old-address deactivation remain in the controller. | Change a delivery address, including campus location and building. Confirm a new active address replaces the old one without changing historical orders. Check unchanged, duplicate-active and matching-inactive cases. |
| AM6 | `Purchasing.Mvc/Controllers/AutoApprovalController.cs`, `TransferValues` | `CopyAutoApproval` updates rule values and selected user/account references, preserving the rule ID and owner. | Edit amount, comparison flags, selected user and expiration. Clear an optional expiration. Verify ownership, validation and access restrictions. Confirm the rule still applies to the intended requester and amount. |
| AM7 | `Purchasing.Mvc/Controllers/CustomFieldController.cs`, `TransferValues` | `CopyCustomField` updates name, rank, required and active flags, preserving ID and organization. | Create and edit a custom field, reorder it, and change required/active flags. Verify its organization and display/validation on an order. |
| AM8 | `Purchasing.Mvc/Controllers/ServiceMessageController.cs`, `TransferValues` | `CopyServiceMessage` updates text, dates, critical and active flags, preserving ID. Existing cache invalidation remains in the controller. | Create and edit a service message, clear its end date, and toggle critical/active. Verify display timing and immediate visibility of edits after cache invalidation. |

### Relationship handling

The old same-type profiles could recursively copy users, roles, organizations and workgroup accounts, even though those entities have their own persistence and edit workflows. The replacement deliberately preserves database identity and tracked workgroup collections. It does not clone related users or organizations. Selected user/account/building references remain the existing entities supplied by model binding.

Workgroup create/edit owns primary and additional organization selection; `CopyWorkgroup` does not modify either relationship. The existing permission-update workflow still runs after setting changes. This distinction must be checked with a populated test workgroup, not just empty fixtures.

## Framework and dependency changes

All 14 C# projects target `net10.0`. [global.json](../global.json) owns the SDK version and roll-forward policy. Azure Pipelines reads that file and is configured to restore the local tools, run tests in Release and publish the same web/jobs artifact layout. The devcontainer uses the .NET 10 Noble image, and VS Code launch paths use `net10.0`.

- BundlerMinifier moves from 4.5.15 to 8.0.2, so Release builds no longer require a .NET 6 runtime. Two Razor `#pragma` directives are moved onto separate lines for the newer compiler.
- NHibernate 5.6.2 and FluentNHibernate 3.4.1 retain the current SQL Server provider and mappings. System.Data.SqlClient moves to 4.9.1. No migration or schema change is included.
- Castle Windsor moves to 6.0.0 with Castle.Core 5.2.1. The host uses `Castle.Windsor.MsDependencyInjection` 6.0.0 and its `WindsorServiceProviderFactory`. The existing `Startup.ConfigureContainer` registrations remain. The former adapter's 6.0.0 release compiled but failed on the first HTTP request with `No scope available`; the replacement passes actual HTTP tests. See the [upstream scope issue](https://github.com/castleproject/Windsor/issues/646) and [replacement adapter documentation](https://github.com/volosoft/castle-windsor-ms-adapter).
- ASP.NET Newtonsoft integration and directory services use 10.0.12; Newtonsoft.Json uses 13.0.4. Azure.Storage.Blobs moves to 12.30.0, and NPOI to 2.7.6 for legacy Excel vendor imports.
- Serilog.AspNetCore uses 10.0.0, Serilog.Exceptions 8.4.0, ClientInfo 2.9.0 and the jobs' console sink 6.1.1. User-Agent enrichment keeps the `ClientAgent` property with the newer header API. Elastic APM uses 1.35.0 and its Serilog enricher 9.0.0. Existing Elasticsearch destinations and query/index contracts are unchanged.
- APM uses targeted ASP.NET Core and Azure Storage packages, with explicit subscribers for incoming requests, outgoing HTTP, SQL Client and Blob Storage. Do not restore `Elastic.Apm.NetCoreAll`: its Elasticsearch integration forces the transport to 7.17.5, which rejects Bonsai's Elasticsearch 7.10.2 OSS distribution. NEST, its JSON serializer and `Elasticsearch.Net` resolve to 7.13.2. Elasticsearch-specific APM spans are omitted; request/HTTP/SQL/blob tracing and Serilog correlation remain configured. Merely disabling APM does not remove the incompatible dependency.
- The jobs explicitly reference Microsoft.Extensions.Caching.Memory 10.0.12 to override the older AE SDK's vulnerable transitive dependency. MVC gets the cache implementation from the ASP.NET shared framework. The AE SDK and legacy KFS integration are otherwise unchanged.
- MSTest stays on its compatible 3.x API at 3.11.1, with Test SDK 18.0.1, Moq 4.20.72 and SQLite 1.0.119. ASP.NET TestHost 10.0.12 exercises the real host and Windsor adapter without live credentials or service calls.

## Repeating local checks

Use the SDK selected by `global.json`:

```sh
dotnet tool restore
dotnet build Purchasing.sln --configuration Release
dotnet test Purchasing.Tests/Purchasing.Tests.csproj --configuration Release --no-build
```

On macOS, exclude `RepositoryTests` with `--filter 'FullyQualifiedName!~RepositoryTests'`; their native System.Data.SQLite provider requires Windows. Azure Pipelines runs the unfiltered suite on Windows; results are recorded below. The solution also contains legacy SSDT projects, so run `dotnet list <project.csproj> package --vulnerable --include-transitive` for each C# project rather than running package listing against the solution.

## Automated checks

- Record the .NET 6 baseline before replacement, then run affected controller/service tests after AutoMapper removal.
- Test field-copy contracts, destination IDs and ownership, optional null values, existing collection identity and fresh-record creation.
- Cover workgroup validation failures and organization selection, including previously disabled edit regressions.
- Run the full test suite on Windows for the native SQLite repository tests.
- Build the entire solution and publish the web app and all six jobs in Release on .NET 10.
- Audit direct and transitive packages. Record remaining advisories and their scope rather than suppressing them.
- Check published CSS/JavaScript bundles and each job's DLL, runtime configuration, `run.cmd` and schedule.
- Smoke-test application startup and authentication redirects without executing production jobs.
- Translate the unit-of-measure query used by AE submission through NHibernate's SQL Server provider, including empty, single-unit and repeated-unit inputs. C# 14 can bind array `.Contains` calls to `MemoryExtensions.Contains`, which NHibernate 5.6.2 cannot translate. `QueryOrderUnitsOfMeasure` must use explicit `Enumerable.Contains`; see [Microsoft's compatibility guidance](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/10.0/csharp-overload-resolution) and [NHibernate issue 3651](https://github.com/nhibernate/nhibernate-core/issues/3651). These offline translation tests do not submit a requisition.
- Run `ElasticsearchCompatibilityTests` through the MVC dependency graph. Its local HTTP endpoint reports Elasticsearch 7.10.2 OSS without a product header, and the real order-history service must return an order and send the expected authorization-ID and status filters. This catches incompatible transitive transport upgrades without requiring Bonsai credentials in CI. Confirm live reads against the configured Bonsai backend separately; a standard Elasticsearch 7.17 Docker image does not cover the OSS product-check failure.

## Combined manual acceptance pass

Run this after the final code and package versions are deployed to Azure test. Use designated test users, records, integrations and email destinations. Keep local persona login restricted to Development and personal user secrets.

- [ ] AM1 Search results
- [ ] AM2 Manual and campus suppliers in Workgroup and Wizard
- [ ] AM3 Workgroup creation
- [ ] AM4 Workgroup editing and invalid-form handling
- [ ] AM5 Address replacement and duplicate handling
- [ ] AM6 Auto-approval rules
- [ ] AM7 Custom fields
- [ ] AM8 Service messages and cache invalidation
- [ ] Real CAS sign-in, sign-out, session expiry, return URLs and permission checks for representative roles
- [ ] Development-only login works when opted in and remains unavailable in hosted non-Development environments
- [ ] Order creation, save, submit, approval, reassignment, purchase and completion
- [ ] Attachments upload/download and existing attachments
- [ ] PDF and Excel exports, including non-ASCII content and dates/amounts, plus bulk vendor `.xls` import
- [ ] Aggie Enterprise validation and submission against the test integration, including an order with multiple unit codes and repeated codes. Confirm the outgoing requisition has the correct unit names and reaches the API without an NHibernate translation error.
- [ ] CSS/JavaScript bundles, form validation and browser console on key pages
- [ ] Logging and APM receive expected requests/errors without startup or serialization failures; confirm request, outgoing HTTP, SQL and Blob Storage spans after the targeted APM change
- [ ] EmailNotifications and DailyEmailNotifications produce the expected test messages without duplicates
- [ ] UpdateOrderIndexes and UpdateLookupIndexes produce searchable updated data
- [ ] AggieEnterprise job processes the intended test records and records outcomes
- [ ] CreateOrderIndexes is tested only against disposable test indexes when a full rebuild is needed

Record evidence here; an unchecked item has not passed.

| Item | Commit and environment | Tester and date | Result and evidence |
| --- | --- | --- | --- |
| Manual acceptance | Pending | Pending | Not run |

## Deployment and rollback

The YAML build publishes artifacts. Separate classic Azure DevOps releases deploy the web and jobs artifacts. Runtime configuration must be updated for both applications and their staging slots before using .NET 10 artifacts. Keep the current release artifact and configuration for rollback.

The inspected production apps use 64-bit Windows processes; test uses 32-bit. Align test with production before acceptance. Keep staging jobs disabled and verify slot-specific job settings before activation or swaps, so only one instance processes email, AE records or index updates. Verify schedules and time-zone assumptions.

After cutover, check real application requests, job outcomes and indexed data. A successful deployment or started process alone does not complete acceptance. Restore the previous artifact and runtime configuration if rollback is needed. This upgrade does not change the database schema.

## Verification record

- Before changes, the .NET 6 non-repository suite passed 1,134 tests on macOS; 14 tests were skipped. Native SQLite repository tests require the Windows run.
- After AutoMapper removal, all 434 runnable tests in the affected controller/service suites passed on .NET 6; four existing skips remained. This includes eight new value-transfer tests and the two restored workgroup validation-failure scenarios.
- On .NET 10, five new runtime checks pass: production CAS challenge and protected-page redirect; opted-in Development login rendering and antiforgery rejection; internal and vendor PDF content; and actual `.xls` vendor import into the selected workgroup. The host tests reproduce the failure with the former Windsor 6 adapter and pass with its replacement.
- Release publish succeeds for the web and all six jobs. The job artifacts contain their DLLs, .NET 10 runtime configuration and `run.cmd`; the five scheduled jobs retain their existing `settings.job` schedules. CreateOrderIndexes correctly has no schedule.
- Before the AE query follow-up, the combined .NET 10 Release suite passed 1,149 tests, with zero failures and 14 existing skips on macOS. The entire solution built in Release with zero errors; existing compiler warnings and MSTest modernization warnings remained.
- Final Release publishing and artifact checks pass for the web and all six jobs, including all 26 configured CSS/JavaScript bundles, IIS `web.config`, runtime configurations, launch scripts and schedules. No AutoMapper package remains in the restored dependency graph.
- The final NuGet audit reports zero known vulnerable packages across all 14 C# projects, including transitive dependencies. This is the advisory feed result at verification time, not a guarantee against undisclosed defects.
- Before the AE query follow-up, [Windows CI build 15052](https://dev.azure.com/ucdavis/Purchasing/_build/results?buildId=15052) passed 3,211 tests with 14 not applicable/skipped and zero failures on commit `a8ca80c5`. This included the native SQLite repository suite.
- The AE unit lookup regression was reproduced on .NET 10: all three `AggieEnterpriseServiceTests` cases failed with NHibernate's `Evaluation failure on op_Implicit(value(System.String[]))` before the fix and passed after explicit `Enumerable.Contains`. The tests prepare the actual submission query using NHibernate 5.6.2 and the SQL Server dialect without opening a database connection or calling AE. They establish query translation, not end-to-end submission acceptance.
- After the AE query fix, the macOS Release non-repository suite passes 1,152 tests with zero failures and the same 14 existing skips.
- The Bonsai compatibility regression fails with `UnsupportedProductException` under the former all-inclusive APM dependency and passes with targeted APM packages. The two existing web-host tests also pass. Earlier Elasticsearch smoke testing used the standard 7.17.28 distribution, which did not expose the production backend's 7.10.2 OSS rejection.
- After the Bonsai dependency fix, the macOS Release non-repository suite passes 1,153 tests with zero failures and 14 existing skips. A separate .NET 10 probe referencing MVC's dependencies executes the actual `GetOrderHistory` method against the configured Bonsai server with a nonexistent order ID and receives HTTP 200. It reads no order contents and makes no index writes. NEST and the transport both resolve to 7.13.2; this verifies the live query, not a signed-in browser session or delivery to the hosted APM collector.
- Hosted manual acceptance and Azure runtime configuration remain pending. No live job was executed or cloud configuration changed during local verification.
