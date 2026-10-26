using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Purchasing.Core.Services
{
    public enum SearchBackend { Bonsai, ElasticCloud }

    public sealed class SearchBackendConfiguration
    {
        public SearchBackend ReadBackend { get; }
        public IReadOnlyList<SearchBackend> WriteTargets { get; }
        public IReadOnlyDictionary<SearchBackend, SearchEndpoint> Endpoints { get; }

        public SearchBackendConfiguration(IConfiguration configuration)
        {
            ReadBackend = ParseBackend(configuration["Search:ReadBackend"] ?? "Bonsai");
            var writes = configuration["Search:WriteTargets"] ?? "Bonsai";
            WriteTargets = writes.Equals("Both", StringComparison.OrdinalIgnoreCase)
                ? new[] { SearchBackend.Bonsai, SearchBackend.ElasticCloud }
                : new[] { ParseBackend(writes) };
            Endpoints = WriteTargets.Append(ReadBackend).Distinct().ToDictionary(b => b,
                b => new SearchEndpoint(configuration, b));
        }

        private static SearchBackend ParseBackend(string value)
        {
            if (!Enum.TryParse<SearchBackend>(value, true, out var backend) || !Enum.IsDefined(backend) ||
                !backend.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Search backend must be Bonsai or ElasticCloud; WriteTargets also accepts Both.");
            return backend;
        }
    }

    public sealed class SearchEndpoint
    {
        public SearchBackend Backend { get; }
        public Uri Url { get; }
        public string IndexPrefix { get; }
        public string Username { get; }
        public string Password { get; }
        public string ApiKey { get; }

        public SearchEndpoint(IConfiguration configuration, SearchBackend backend)
        {
            Backend = backend;
            var section = configuration.GetSection($"Search:{backend}");
            var url = section["Url"] ?? (backend == SearchBackend.Bonsai ? configuration["ElasticSearchUrl"] : null);
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "http" && uri.Scheme != "https") ||
                !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
                throw new ArgumentException($"A valid HTTP(S) Search:{backend}:Url is required.");

            Username = section["Username"];
            Password = section["Password"];
            ApiKey = section["ApiKey"];
            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                if (Username != null || Password != null || !string.IsNullOrEmpty(ApiKey))
                    throw new ArgumentException($"Configure only one credential source for {backend}.");
                var credentials = uri.UserInfo.Split(new[] { ':' }, 2);
                Username = Uri.UnescapeDataString(credentials[0]);
                Password = credentials.Length == 2 ? Uri.UnescapeDataString(credentials[1]) : null;
            }
            if ((Username == null) != (Password == null) ||
                (!string.IsNullOrEmpty(ApiKey) && Username != null))
                throw new ArgumentException($"Configure either an API key or a username and password for {backend}.");

            Url = new UriBuilder(uri) { UserName = "", Password = "" }.Uri;
            IndexPrefix = section["IndexPrefix"] ?? "opp";
            if (!Regex.IsMatch(IndexPrefix, "^[a-z0-9][a-z0-9_-]{0,79}$"))
                throw new ArgumentException($"Search:{backend}:IndexPrefix must contain 1 to 80 lowercase letters, digits, hyphens or underscores and start with a letter or digit.");
        }
    }

    public sealed class SearchBackendSet
    {
        public IndexBackend Reader { get; }
        public IReadOnlyList<IndexBackend> Writers { get; }

        public SearchBackendSet(IConfiguration configuration)
        {
            var options = new SearchBackendConfiguration(configuration);
            var backends = options.Endpoints.ToDictionary(e => e.Key, e => e.Key == SearchBackend.Bonsai
                ? (IndexBackend)new BonsaiIndexBackend(e.Value)
                : new ElasticCloudIndexBackend(e.Value));
            Reader = backends[options.ReadBackend];
            Writers = options.WriteTargets.Select(b => backends[b]).ToArray();
        }

        internal SearchBackendSet(IndexBackend reader, params IndexBackend[] writers)
        {
            Reader = reader;
            Writers = writers;
        }
    }
}
