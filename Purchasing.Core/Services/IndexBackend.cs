using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;
using Serilog;

namespace Purchasing.Core.Services
{
    public sealed class SearchBackendException : Exception
    {
        public SearchBackendException(string message) : base(message) { }
    }

    // Both transports use the same Elasticsearch JSON contract. Domain documents retain
    // Newtonsoft attributes, including inherited opt-in serialization, on both versions.
    public abstract class IndexBackend
    {
        internal static readonly JsonSerializerSettings DocumentSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Include,
            DateParseHandling = DateParseHandling.DateTime
        };
        public string Name { get; }
        private readonly string _prefix;

        protected IndexBackend(string name, string prefix)
        {
            Name = name;
            _prefix = prefix;
        }

        public string IndexName(Indexes index) => $"{_prefix}-{index.ToString().ToLowerInvariant()}";
        protected abstract (int? Status, string Body) Send(string method, string path, string body);

        internal JObject Request(string method, string path, object body = null, bool allowNotFound = false)
        {
            (int? Status, string Body) response;
            try
            {
                response = Send(method, path, body is string text ? text :
                    body == null ? null : JsonConvert.SerializeObject(body));
            }
            catch (Exception exception)
            {
                // Transport exceptions/debug output may contain credentials or document payloads.
                throw new SearchBackendException($"{Name}: {method} {path} could not complete ({exception.GetType().Name}).");
            }
            if (allowNotFound && response.Status == 404) return null;
            if (response.Status < 200 || response.Status >= 300 || !response.Status.HasValue)
                throw new SearchBackendException($"{Name}: {method} {path} failed with HTTP {response.Status?.ToString() ?? "unavailable"}.");
            try
            {
                var result = string.IsNullOrWhiteSpace(response.Body) ? new JObject() : JObject.Parse(response.Body);
                if (result["error"] != null || result.Value<bool?>("timed_out") == true ||
                    result["_shards"]?.Value<int?>("failed") > 0 || result.Value<bool?>("acknowledged") == false)
                    throw new SearchBackendException($"{Name}: {method} {path} returned an incomplete or failed response.");
                return result;
            }
            catch (JsonException)
            {
                throw new SearchBackendException($"{Name}: {method} {path} returned invalid JSON.");
            }
        }

        internal JObject Search(Indexes index, JObject query)
        {
            var result = Request("POST", $"/{IndexName(index)}/_search?allow_partial_search_results=false", query);
            if (result["hits"]?["hits"] is not JArray)
                throw new SearchBackendException($"{Name}: search returned no hits envelope.");
            return result;
        }

        internal static List<T> Hits<T>(JObject result) => result["hits"]["hits"]
            .Select(h => h["_source"].ToObject<T>(JsonSerializer.Create(DocumentSettings))).ToList();

        public int Count(Indexes index)
        {
            var result = Request("GET", $"/{IndexName(index)}/_count");
            if (result["count"]?.Type != JTokenType.Integer)
                throw new SearchBackendException($"{Name}: count returned no document count for {index}.");
            return checked((int)result.Value<long>("count"));
        }

        public void Write<T>(Indexes index, IEnumerable<T> documents, bool recreate = false) where T : class
        {
            var alias = IndexName(index);
            var destination = recreate ? $"{alias}-{Guid.NewGuid():N}" : alias;
            if (recreate)
                Request("PUT", $"/{destination}", Mapping<T>());
            else
                EnsureIndex<T>(alias);

            var errors = new IndexingFailures();
            foreach (var batch in documents.Partition(1000))
                errors.Attempt(() => WriteBatch(destination, batch), $"{Name}/{index} bulk");
            errors.ThrowIfAny();

            if (!recreate) return;
            Request("POST", $"/{destination}/_refresh");
            // Only promote a fully written replacement. An existing concrete legacy index
            // is replaced atomically; previous generations are removed after promotion.
            var existing = Request("GET", $"/{alias}/_alias", allowNotFound: true);
            var actions = new JArray();
            if (existing != null)
            {
                foreach (var property in existing.Properties())
                {
                    if (property.Name == alias)
                        actions.Add(new JObject { ["remove_index"] = new JObject { ["index"] = alias } });
                    else if (property.Value["aliases"]?[alias] != null)
                        actions.Add(new JObject { ["remove"] = new JObject { ["index"] = property.Name, ["alias"] = alias } });
                }
            }
            actions.Add(new JObject { ["add"] = new JObject { ["index"] = destination, ["alias"] = alias, ["is_write_index"] = true } });
            var promotion = Request("POST", "/_aliases", new JObject { ["actions"] = actions });
            if (promotion.Value<bool?>("errors") == true)
                throw new SearchBackendException($"{Name}: alias promotion failed for {alias}.");
            // Daily lookup rebuilds must not accumulate a new index indefinitely. Only
            // remove our prior generations after promotion, and retain any shared alias.
            if (existing != null)
                foreach (var previous in existing.Properties().Where(p => p.Name != alias &&
                             p.Name.StartsWith(alias + "-", StringComparison.Ordinal)))
                {
                    var aliases = Request("GET", $"/{previous.Name}/_alias", allowNotFound: true);
                    if (aliases?[previous.Name]?["aliases"] is JObject names && !names.HasValues)
                        Request("DELETE", $"/{previous.Name}");
                }
        }

        public void Reconcile<T>(Indexes index, int[] orderIds, IEnumerable<T> documents) where T : class
        {
            if (orderIds.Length == 0) return;
            var errors = new IndexingFailures();
            errors.Attempt(() => EnsureIndex<T>(IndexName(index)), $"{Name}/{index} ensure index", orderIds: orderIds);
            // Refresh before deletion so a repeat run sees the previous run's writes.
            errors.Attempt(() => Request("POST", $"/{IndexName(index)}/_refresh"), $"{Name}/{index} refresh", orderIds: orderIds);
            errors.Attempt(() =>
            {
                var result = Request("POST", $"/{IndexName(index)}/_delete_by_query?refresh=true",
                    new { query = new { terms = new { orderId = orderIds } } });
                if (result["failures"]?.Any() == true || result.Value<long?>("version_conflicts") > 0)
                    throw new SearchBackendException($"{Name}: deleting stale {index} documents failed.");
            }, $"{Name}/{index} delete", orderIds: orderIds);
            errors.Attempt(() => Write(index, documents), $"{Name}/{index} replacements", orderIds: orderIds);
            errors.ThrowIfAny();
        }

        private void EnsureIndex<T>(string index)
        {
            if (Request("HEAD", $"/{index}", allowNotFound: true) == null)
                Request("PUT", $"/{index}", Mapping<T>());
        }

        private void WriteBatch<T>(string index, IEnumerable<T> documents) where T : class
        {
            var body = new StringBuilder();
            var submitted = new List<Dictionary<string, object>>();
            var errors = new IndexingFailures();
            foreach (var document in documents)
            {
                var identity = new Dictionary<string, object>();
                errors.Attempt(() =>
                {
                    try
                    {
                        var contract = (JsonObjectContract)DocumentSettings.ContractResolver.ResolveContract(document.GetType());
                        var idProperty = document is OrderHistory || document is OrderTrackingEntity ? "orderId" : "id";
                        identity["DocumentId"] = contract.Properties.GetProperty(idProperty, StringComparison.Ordinal)?.ValueProvider.GetValue(document);
                        identity["OrderId"] = contract.Properties.GetProperty("orderId", StringComparison.Ordinal)?.ValueProvider.GetValue(document);
                        var source = JObject.FromObject(document, JsonSerializer.Create(DocumentSettings));
                        var id = source[idProperty];
                        if (id == null || string.IsNullOrWhiteSpace(id.ToString()))
                            throw new SearchBackendException($"{Name}/{index}: document has no stable ID.");
                        // SQL's history Id is a row number that changes with batch boundaries.
                        if (document is OrderHistory) source["id"] = id.DeepClone();
                        var metadata = new JObject { ["index"] = new JObject { ["_index"] = index, ["_id"] = id.ToString() } };
                        var serialized = source.ToString(Formatting.None);
                        body.AppendLine(metadata.ToString(Formatting.None));
                        body.AppendLine(serialized);
                        submitted.Add(identity);
                    }
                    catch (Exception exception)
                    {
                        throw new SearchBackendException($"{Name}/{index}: document serialization failed ({exception.GetType().Name}).");
                    }
                }, $"{Name}/{index} serialization", new[] { identity });
            }
            if (body.Length > 0)
            {
                errors.Attempt(() =>
                {
                    Log.Information("Indexing {DocumentCount} documents into {Backend}/{Index}", submitted.Count, Name, index);
                    var result = Request("POST", "/_bulk", body.ToString());
                    if (result["items"] is not JArray items || items.Count != submitted.Count)
                        throw new SearchBackendException($"{Name}/{index}: bulk response does not account for every submitted document.");
                    var failed = false;
                    foreach (var item in items.Select(i => ((JObject)i).Properties().Single().Value))
                    {
                        if (item.Value<int>("status") < 200 || item.Value<int>("status") >= 300 || item["error"] != null)
                        {
                            failed = true;
                            Log.Error("Indexing failed for {Backend}/{Index}, document {DocumentId}, status {Status}, type {ErrorType}",
                                Name, index, item.Value<string>("_id"), item.Value<int>("status"), item["error"]?["type"]?.ToString());
                        }
                    }
                    if (failed || result.Value<bool?>("errors") == true)
                        throw new SearchBackendException($"{Name}/{index}: bulk indexing failed for one or more documents.");
                }, $"{Name}/{index} bulk request", submitted);
            }
            errors.ThrowIfAny();
        }

        internal static JObject Mapping<T>()
        {
            var contract = (JsonObjectContract)DocumentSettings.ContractResolver.ResolveContract(typeof(T));
            var properties = new JObject();
            foreach (var property in contract.Properties.Where(p => !p.Ignored && p.Readable))
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (type == typeof(string))
                    properties[property.PropertyName] = new JObject { ["type"] = "text", ["fields"] = new JObject { ["keyword"] = new JObject { ["type"] = "keyword", ["ignore_above"] = 256 } } };
                else if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
                    properties[property.PropertyName] = new JObject { ["type"] = "date", ["format"] = "strict_date_optional_time||epoch_millis" };
                else if (type == typeof(bool)) properties[property.PropertyName] = new JObject { ["type"] = "boolean" };
                else if (type == typeof(int) || type == typeof(long)) properties[property.PropertyName] = new JObject { ["type"] = "long" };
                else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) properties[property.PropertyName] = new JObject { ["type"] = "double" };
            }
            return new JObject
            {
                ["settings"] = new JObject { ["index.max_terms_count"] = 1000000 },
                ["mappings"] = new JObject { ["dynamic"] = false, ["properties"] = properties }
            };
        }
    }

    public sealed class IndexingFailures
    {
        private readonly List<Exception> _errors = new List<Exception>();
        public void Attempt(Action action, string operation, object documents = null, int[] orderIds = null)
        {
            try { action(); }
            catch (Exception exception)
            {
                Log.Error(exception, "Indexing operation failed: {Operation}; order IDs {OrderIds}; documents {@Documents}",
                    operation, orderIds, documents);
                _errors.Add(exception);
            }
        }
        public void ThrowIfAny()
        {
            if (_errors.Count > 0) throw new AggregateException("One or more indexing operations failed. Review the job log for affected targets and documents.", _errors);
        }
        public static void RunAll(params Action[] operations)
        {
            var failures = new IndexingFailures();
            foreach (var operation in operations) failures.Attempt(operation, operation.Method.Name);
            failures.ThrowIfAny();
        }
    }
}
