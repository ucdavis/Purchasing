using System;
using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;

namespace Purchasing.Core.Services
{
    public sealed class BonsaiIndexBackend : IndexBackend
    {
        private readonly ElasticClient _client;
        public BonsaiIndexBackend(SearchEndpoint endpoint) : base("Bonsai", endpoint.IndexPrefix)
        {
            // Keep the 7.13.2 NEST transport: later versions reject Bonsai's 7.10.2 OSS response.
            var settings = new ConnectionSettings(new SingleNodeConnectionPool(endpoint.Url), JsonNetSerializer.Default)
                .RequestTimeout(endpoint.RequestTimeout);
            if (endpoint.Username != null) settings.BasicAuthentication(endpoint.Username, endpoint.Password);
            if (!string.IsNullOrEmpty(endpoint.ApiKey)) settings.ApiKeyAuthentication(new ApiKeyAuthenticationCredentials(endpoint.ApiKey));
            _client = new ElasticClient(settings);
        }

        protected override (int? Status, string Body) Send(string method, string path, string body)
        {
            // The legacy transport requires query parameters separately from the path.
            var parts = path.Split(new[] { '?' }, 2);
            var parameters = new LegacyRequestParameters(Enum.Parse<HttpMethod>(method));
            if (parts.Length == 2)
                foreach (var parameter in parts[1].Split('&'))
                {
                    var pair = parameter.Split(new[] { '=' }, 2);
                    parameters.SetQueryString(pair[0], pair.Length == 2 ? pair[1] : "");
                }
            var response = _client.LowLevel.DoRequest<StringResponse>(Enum.Parse<HttpMethod>(method), parts[0],
                body == null ? null : PostData.String(body), parameters);
            return (response.HttpStatusCode, response.Body);
        }

        private sealed class LegacyRequestParameters : RequestParameters<LegacyRequestParameters>
        {
            public LegacyRequestParameters(HttpMethod method) { DefaultHttpMethod = method; }
            public override HttpMethod DefaultHttpMethod { get; }
            public override bool SupportsBody => true;
        }
    }

    public sealed class ElasticCloudIndexBackend : IndexBackend
    {
        private readonly Elastic.Clients.Elasticsearch.ElasticsearchClient _client;
        public ElasticCloudIndexBackend(SearchEndpoint endpoint) : base("ElasticCloud", endpoint.IndexPrefix)
        {
            var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(endpoint.Url)
                .RequestTimeout(endpoint.RequestTimeout);
            if (endpoint.Username != null)
                settings.Authentication(new Elastic.Transport.BasicAuthentication(endpoint.Username, endpoint.Password));
            if (!string.IsNullOrEmpty(endpoint.ApiKey))
                settings.Authentication(new Elastic.Transport.ApiKey(endpoint.ApiKey));
            _client = new Elastic.Clients.Elasticsearch.ElasticsearchClient(settings);
        }

        protected override (int? Status, string Body) Send(string method, string path, string body)
        {
            var response = _client.Transport.Request<Elastic.Transport.StringResponse>(
                new Elastic.Transport.EndpointPath(Enum.Parse<Elastic.Transport.HttpMethod>(method), path),
                body == null ? null : Elastic.Transport.PostData.String(body), null, null);
            return (response.ApiCallDetails.HttpStatusCode, response.Body);
        }
    }
}
