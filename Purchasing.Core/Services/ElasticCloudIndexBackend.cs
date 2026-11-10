using System;

namespace Purchasing.Core.Services
{
    public sealed class ElasticCloudIndexBackend : IndexBackend
    {
        private readonly Elastic.Clients.Elasticsearch.ElasticsearchClient _client;
        public ElasticCloudIndexBackend(SearchEndpoint endpoint) : base("ElasticCloud", endpoint.IndexPrefix)
        {
            var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(endpoint.Url)
                .RequestTimeout(TimeSpan.FromSeconds(60));
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
