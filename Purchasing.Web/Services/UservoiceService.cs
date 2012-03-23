using System.Linq;
using System.Net;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;
using Purchasing.Web.Helpers;

namespace Purchasing.Web.Services
{
    //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/suggestions.json?category=31579&sort=newest";
    //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/categories/31577.json";
    //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/categories.json";
    //const string query = "https://ucdavis.uservoice.com/api/v1/users/24484752.json";

    public interface IUservoiceService
    {
        int GetActiveIssuesCount();
        string GetOpenIssues();
    }

    /// <summary>
    /// Encapsulates API calls to uservoice using OAuth
    /// </summary>
    public class UservoiceService : IUservoiceService
    {
        private static readonly string ApiKey = WebConfigurationManager.AppSettings["uservoiceKey"];
        private static readonly string ApiSecret = WebConfigurationManager.AppSettings["uservoiceSecret"];
        private const string ApiUrlBase = "https://ucdavis.uservoice.com";
        private const string ForumId = "126891";
        private const string IssuesCategoryId = "31579";

        public string GetOpenIssues()
        {
            string endpoint = CreateEndpoint("/api/v1/forums/{0}/suggestions.json?category={1}&sort=newest&per_page=100");

            return PerformQuery(endpoint);
        }

        public int GetActiveIssuesCount()
        {
            string endpoint = CreateEndpoint("/api/v1/forums/{0}/categories.json");

            var result = JObject.Parse(PerformQuery(endpoint));
            
            var issueCategory =
                result["categories"].Children().Single(c => c["name"].Value<string>() == "Issues");

            return issueCategory["suggestions_count"].Value<int>();
        }

        /// <summary>
        /// Performs a query against the ucdavis prepurchasing uservoice using the desired endpoint and http method
        /// </summary>
        /// <param name="endpoint">/api/... </param>
        /// <param name="method">GET/POST/PUT/DELETE</param>
        /// <remarks>http://developer.uservoice.com/docs/api-public/</remarks>
        /// <returns>Result string from API call</returns>
        private string PerformQuery(string endpoint, string method = "GET")
        {
            var query = ApiUrlBase + endpoint;

            var oauth = new Manager();
            oauth["consumer_key"] = ApiKey;
            oauth["consumer_secret"] = ApiSecret;

            var header = oauth.GenerateAuthzHeader(query, method);

            var req = WebRequest.Create(query);
            req.Headers.Add("Authorization", header);

            using (var response = (HttpWebResponse)req.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Pass in a tokenized string with {0} to be replaced with forumId, {1} replaced with the issues category id
        /// </summary>
        private string CreateEndpoint(string tokenizedEndpoint)
        {
            return string.Format(tokenizedEndpoint, ForumId, IssuesCategoryId);
        }
    }
}