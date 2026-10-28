using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Purchasing.Core.Services
{
    internal interface ISearchBackfillTarget
    {
        string Identity { get; }
        void InspectIsolation();
        void Write(SearchProjection projection, int[] orderIds = null);
        void Refresh(Indexes index);
        IReadOnlyList<(string Id, JObject Source)> Get(Indexes index, string[] ids);
        IEnumerable<IReadOnlyList<(string Id, JObject Source)>> Scan(Indexes index, int size);
        void Delete(Indexes index, string[] ids);
    }

    internal sealed class SearchBackfillTarget : ISearchBackfillTarget
    {
        private readonly IndexBackend _backend;
        public string Identity { get; }
        public SearchBackfillTarget(IndexBackend backend, SearchEndpoint endpoint)
        {
            _backend = backend;
            // Do not persist credentials or URLs with access-bearing path/query components.
            if (endpoint.Backend != SearchBackend.ElasticCloud || endpoint.Url.AbsolutePath != "/")
                throw new ArgumentException("The runner requires a root Elastic Cloud endpoint.");
            Identity = endpoint.Url.GetLeftPart(UriPartial.Authority) + "/" + endpoint.IndexPrefix;
        }

        public void InspectIsolation()
        {
            foreach (var index in Enum.GetValues<Indexes>())
            {
                var name = _backend.IndexName(index);
                var resolved = _backend.Request("GET", $"/{name}/_alias", allowNotFound: true);
                if (resolved == null) continue;
                if (resolved.Count != 1) throw new BackfillSafetyException($"{index}: expected one physical index.");
                var physical = resolved.Properties().Single();
                if (physical.Name != name && !physical.Name.StartsWith(name + "-", StringComparison.Ordinal))
                    throw new BackfillSafetyException($"{index}: alias points outside its Purchasing namespace.");
                if (physical.Value["aliases"] is not JObject aliases ||
                    aliases.Properties().Any(a => a.Name != name || a.Value["filter"] != null ||
                        a.Value["routing"] != null || a.Value["index_routing"] != null || a.Value["search_routing"] != null) ||
                    (physical.Name != name && aliases[name] == null))
                    throw new BackfillSafetyException($"{index}: shared, filtered, or routed aliases are not supported.");
            }
        }

        public void Write(SearchProjection projection, int[] orderIds = null) => projection.Write(_backend, orderIds);
        public void Refresh(Indexes index) => _backend.Request("POST", $"/{_backend.IndexName(index)}/_refresh");

        public IReadOnlyList<(string Id, JObject Source)> Get(Indexes index, string[] ids)
        {
            var response = _backend.Request("POST", $"/{_backend.IndexName(index)}/_mget", new { ids }, preserveSourceValues: true);
            if (response["docs"] is not JArray docs || docs.Count != ids.Length)
                throw new SearchBackendException($"{index}: incomplete multi-get response.");
            var result = new List<(string, JObject)>();
            for (var i = 0; i < ids.Length; i++)
            {
                var doc = docs[i];
                if (doc.Value<string>("_id") != ids[i] || doc["error"] != null || doc["found"]?.Type != JTokenType.Boolean ||
                    (doc.Value<bool>("found") && doc["_source"] is not JObject))
                    throw new SearchBackendException($"{index}: failed or malformed multi-get item.");
                if (doc.Value<bool>("found")) result.Add((ids[i], (JObject)doc["_source"]));
            }
            return result;
        }

        public IEnumerable<IReadOnlyList<(string Id, JObject Source)>> Scan(Indexes index, int size)
        {
            string scroll = null;
            try
            {
                var result = _backend.Request("POST", $"/{_backend.IndexName(index)}/_search?scroll=5m&allow_partial_search_results=false",
                    new { size, sort = new[] { "_doc" }, query = new { match_all = new { } } }, preserveSourceValues: true);
                while (true)
                {
                    scroll = result.Value<string>("_scroll_id") ?? scroll;
                    if (string.IsNullOrEmpty(scroll) || result["hits"]?["hits"] is not JArray hits || hits.Count > size)
                        throw new SearchBackendException($"{index}: incomplete scan response.");
                    if (hits.Count == 0) yield break;
                    var page = new List<(string, JObject)>();
                    foreach (var hit in hits)
                    {
                        if (string.IsNullOrEmpty(hit.Value<string>("_id")) || hit["_source"] is not JObject source)
                            throw new SearchBackendException($"{index}: scan returned an invalid document.");
                        page.Add((hit.Value<string>("_id"), source));
                    }
                    yield return page;
                    result = _backend.Request("POST", "/_search/scroll", new { scroll = "5m", scroll_id = scroll }, preserveSourceValues: true);
                }
            }
            finally
            {
                if (scroll != null)
                {
                    var cleared = _backend.Request("DELETE", "/_search/scroll", new { scroll_id = new[] { scroll } });
                    if (cleared.Value<bool?>("succeeded") != true)
                        throw new SearchBackendException($"{index}: scan cleanup failed.");
                }
            }
        }

        public void Delete(Indexes index, string[] ids)
        {
            if (ids.Length == 0) return;
            var result = _backend.Request("POST", $"/{_backend.IndexName(index)}/_delete_by_query?refresh=true",
                new { query = new { ids = new { values = ids } } });
            if (result["failures"] is not JArray failures || failures.Count != 0 || result.Value<long?>("version_conflicts") != 0 ||
                result.Value<long?>("deleted") != ids.Length)
                throw new SearchBackendException($"{index}: unexpected-document deletion was incomplete.");
        }

        internal static bool SameContent(JObject expected, JObject actual) => JToken.DeepEquals(Normalize(expected), Normalize(actual));

        private static JToken Normalize(JToken source)
        {
            // Compare JSON values, including integer/decimal equivalence, without losing field/array contents.
            var parsed = JsonConvert.DeserializeObject<JToken>(source.ToString(Formatting.None), new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None, FloatParseHandling = FloatParseHandling.Decimal
            });
            JToken Visit(JToken token) => token switch
            {
                JObject obj => new JObject(obj.Properties().OrderBy(p => p.Name, StringComparer.Ordinal).Select(p => new JProperty(p.Name, Visit(p.Value)))),
                JArray array => new JArray(array.Select(Visit)),
                JValue value when value.Type == JTokenType.Integer || value.Type == JTokenType.Float =>
                    new JValue(Convert.ToDecimal(value.Value, CultureInfo.InvariantCulture)),
                _ => token.DeepClone()
            };
            return Visit(parsed);
        }
    }
}
