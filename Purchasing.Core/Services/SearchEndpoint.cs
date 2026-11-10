using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Purchasing.Core.Models.Configuration;

namespace Purchasing.Core.Services
{
    public sealed class SearchEndpoint
    {
        public Uri Url { get; }
        public string IndexPrefix { get; }
        public string Username { get; }
        public string Password { get; }
        public string ApiKey { get; }

        public SearchEndpoint(IConfiguration configuration)
        {
            var search = configuration.GetSection("Search:ElasticCloud")
                .Get<ElasticCloudSearchOptions>() ?? new ElasticCloudSearchOptions();
            if (!Uri.TryCreate(search.Url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "http" && uri.Scheme != "https") ||
                !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
                throw new ArgumentException("A valid HTTP(S) Search:ElasticCloud:Url is required.");

            Username = search.Username;
            Password = search.Password;
            ApiKey = search.ApiKey;
            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                if (Username != null || Password != null || !string.IsNullOrEmpty(ApiKey))
                    throw new ArgumentException("Configure only one credential source for ElasticCloud.");
                var credentials = uri.UserInfo.Split(new[] { ':' }, 2);
                Username = Uri.UnescapeDataString(credentials[0]);
                Password = credentials.Length == 2 ? Uri.UnescapeDataString(credentials[1]) : null;
            }
            if ((Username == null) != (Password == null) ||
                (!string.IsNullOrEmpty(ApiKey) && Username != null))
                throw new ArgumentException("Configure either an API key or a username and password for ElasticCloud.");

            Url = new UriBuilder(uri) { UserName = "", Password = "" }.Uri;
            IndexPrefix = search.IndexPrefix ?? "opp";
            if (!Regex.IsMatch(IndexPrefix, "^[a-z0-9][a-z0-9_-]{0,79}$"))
                throw new ArgumentException("Search:ElasticCloud:IndexPrefix must contain 1 to 80 lowercase letters, digits, hyphens or underscores and start with a letter or digit.");
        }
    }

}
