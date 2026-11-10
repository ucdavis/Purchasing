# Logging, tracing, and metrics

The web application and jobs write structured logs through Serilog. Console output remains available in App Service and WebJob logs. `Serilog.Sinks.OpenTelemetry` also sends events to an OTLP receiver, which forwards them to Elastic. The OpenTelemetry SDK exports traces and metrics to the same receiver. Search uses its own Elasticsearch client and configuration.

Only Serilog exports application logs. Registering an additional OpenTelemetry logging provider would duplicate them. Exporter requests are excluded from tracing to avoid tracing telemetry delivery itself.

## Configuration

Set these values in App Service application settings, local environment variables, or development user secrets. Each process reads them at startup. MVC resolves the log, trace, and metric settings from the same final host configuration. Credentials belong in secret configuration.

| Setting | Purpose |
| --- | --- |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OTLP receiver URL. Use the receiver's ingestion endpoint, not the Elasticsearch search URL. No embedded credentials, query, or fragment. |
| `OTEL_EXPORTER_OTLP_PROTOCOL` | `grpc`, the default, or `http/protobuf`. Match the receiver. |
| `OTEL_EXPORTER_OTLP_HEADERS` | Authentication headers, for example `Authorization=ApiKey <secret>` or the authentication scheme required by the receiver. Comma-separated values support percent encoding. |
| `OTEL_SERVICE_NAME` | Service identity. Defaults to `purchasing.web` for MVC and the entry assembly name for each job. |
| `OTEL_RESOURCE_ATTRIBUTES` | Comma-separated attributes, for example `service.namespace=purchasing,deployment.environment.name=Test`. The namespace defaults to `purchasing`; environment defaults to `ASPNETCORE_ENVIRONMENT`, then `Production`. |
| `OTEL_TRACES_SAMPLER`, `OTEL_TRACES_SAMPLER_ARG` | Standard OpenTelemetry SDK sampling configuration. The default respects the parent's sampling decision and samples all root traces. Use `parentbased_traceidratio` and a ratio such as `0.2` to reduce root trace volume. |

The `LOGS`, `TRACES`, and `METRICS` variants of the endpoint, protocol, and headers settings override the shared values. For example, `OTEL_EXPORTER_OTLP_LOGS_ENDPOINT` changes only logging. With HTTP/protobuf, the shared endpoint receives `/v1/logs`, `/v1/traces`, or `/v1/metrics`; a signal-specific endpoint must already contain its full path. For gRPC, supply the receiver address without an HTTP signal path.

Without an endpoint for a signal, that signal is not exported. Local console logging still works. Configure endpoints and authentication for both web and jobs before deploying an environment that depends on remote telemetry. These settings replace the previous logging endpoint and Elastic APM configuration; those old settings no longer enable delivery.

Web and job hosts can use different service names. If all jobs share one `OTEL_SERVICE_NAME`, their root spans still identify the executable in `job.name`.

## Context and correlation

Logs retain structured message properties, exception details, `Application`, and `AppEnvironment`. The latter two come from the telemetry service name and `deployment.environment.name`. Web logs also include the existing user, client IP, user agent, session, endpoint, and response content-type enrichment.

`RequestId` is the ASP.NET request identifier. OTLP's trace and span IDs come from the current .NET activity and correlate a log with its trace. Logs outside an activity have no trace correlation. MVC's bootstrap logger writes only to the console until the host configures the final logger; those early events are not exported later.

Web tracing covers ASP.NET Core requests, outgoing HTTP calls, SQL commands, and Azure Blob SDK operations. Each job starts a root activity for its run, so SQL and HTTP work and job logs share a trace. HTTP and SQL instrumentation produce metrics; web hosts also collect ASP.NET Core metrics. Sampling affects exported traces, not whether Serilog writes a log.

## Delivery and failures

Export happens in batches. MVC disposes its host and closes Serilog on shutdown. Every job enters the common run wrapper, finishes its root span, disposes the tracing and metrics providers, and closes Serilog before exiting. An unhandled job failure is logged, marks the root span as failed, and still runs shutdown cleanup.

Exporter failures go to standard error with sanitized diagnostics. Check authentication, receiver permissions, network reachability, and ingestion health when these appear. A receiver accepting a batch does not prove Elastic indexed it; check the stored records as well.

Queues are in memory. Forced termination, a sustained receiver outage, or a shutdown that exceeds the platform's timeout can lose pending telemetry. Console output remains useful during an exporter outage. Existing job-specific error handling still determines whether a caught business error causes a failed job exit.

## Finding data in Elastic

Select the service name and deployment environment in Elastic Observability. The receiver determines the data streams and field mapping. The Elastic APM OTLP endpoint maps telemetry into APM/ECS fields; other receivers can store native OTLP fields. Verify actual documents before updating queries or dashboards.

With Elastic APM intake, application logs use `logs-apm.app.<service.name>-<namespace>` and exception records use `logs-apm.error-<namespace>`. Traces use `traces-apm-<namespace>` and application metrics use `metrics-apm.app.<service.name>-<namespace>`. Receiver configuration can reroute these streams. The namespace does not automatically follow the application's deployment environment. See [APM data streams](https://www.elastic.co/docs/solutions/observability/apm/data-streams) for naming and routing rules.

Existing `aspnet-purchasing-*` indexes remain historical data. New OTLP events do not continue those monthly indexes. Data views, saved searches, alerts, and retention must cover the receiver's destination streams. A view that searches only `aspnet-purchasing-*` will not show new events.

Update saved queries for the receiver's stored field names. Elastic's [application log view](https://www.elastic.co/docs/solutions/observability/apm/logs) supports correlation through `trace.id`; the HTTP `RequestId` identifies a request, not a distributed trace. Custom log properties can appear in `_source` without searchable mappings because APM application logs disable dynamic mapping. Check mappings before relying on filters for `Application`, `AppEnvironment`, or order IDs; see [OTLP intake limitations](https://www.elastic.co/docs/solutions/observability/apm/opentelemetry/limitations#opentelemetry-logs).

For an environment change, verify a web request and a short job produce stored logs, correlated traces, and metrics. Check an exception, the service/environment fields, and the receiver's retention policy. Keep historical views available when switching dashboards. To roll back the application, restore the previous build and its logging/APM settings; that build does not understand this OTLP configuration.

## Local verification

Run the behavioral tests with:

```sh
dotnet test Purchasing.Tests/Purchasing.Tests.csproj --filter FullyQualifiedName~TelemetryTests
```

The tests run a loopback OTLP receiver and decode the exported protobuf contract. They cover web and job logging over gRPC and HTTP, trace correlation, structured exceptions, HTTP metrics, Blob spans, signal-specific configuration, failure diagnostics, and flushing. They do not need cloud credentials.
