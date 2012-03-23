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

        public int GetActiveIssuesCount()
        {
            string endpoint = CreateEndpoint("/api/v1/forums/{0}/categories.json");

            var result = PerformQuery(endpoint);
            
            var issueCategory =
                result["categories"].Children().Single(c => c["name"].Value<string>() == "Issues");

            return issueCategory["suggestions_count"].Value<int>();
        }

        private JObject PerformQuery(string endpoint, string method = "GET")
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
                    return JObject.Parse(reader.ReadToEnd());
                }
            }
        }

        private string CreateEndpoint(string tokenizedEndpoint)
        {
            return string.Format(tokenizedEndpoint, ForumId);
        }
    }
}