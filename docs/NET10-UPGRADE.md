# .NET 10 upgrade and manual testing plan

This upgrade removes AutoMapper, then updates the web app, libraries, tests and six scheduled jobs to .NET 10. The final acceptance pass covers the combined change. The application keeps MVC, NHibernate, its existing SQL schema and Windows App Service hosting.

## AutoMapper replacement

The eight active calls are replaced by typed field assignments in `Purchasing.Core/Helpers/EditableValues.cs` and `SearchResults.OrderResult.FromHistory`. AutoMapper's package, profiles, container registration and injected `IMapper` parameters are removed. Test fixtures no longer initialize AutoMapper.

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

## Automated checks

- Record the .NET 6 baseline before replacement, then run affected controller/service tests after AutoMapper removal.
- Test field-copy contracts, destination IDs and ownership, optional null values, existing collection identity and fresh-record creation.
- Cover workgroup validation failures and organization selection, including previously disabled edit regressions.
- Run the full test suite on Windows for the native SQLite repository tests.
- Build the entire solution and publish the web app and all six jobs in Release on .NET 10.
- Audit direct and transitive packages. Record remaining advisories and their scope rather than suppressing them.
- Check published CSS/JavaScript bundles and each job's DLL, runtime configuration, `run.cmd` and schedule.
- Smoke-test application startup and authentication redirects without executing production jobs.

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
- [ ] PDF and Excel exports, including non-ASCII content and dates/amounts
- [ ] Aggie Enterprise validation and submission against the test integration
- [ ] CSS/JavaScript bundles, form validation and browser console on key pages
- [ ] Logging and APM receive expected requests/errors without startup or serialization failures
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
- Further automated results and final package decisions will be recorded after the combined code changes.
