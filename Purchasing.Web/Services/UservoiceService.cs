using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;
using Purchasing.Web.Helpers;
using System.Collections.Generic;

namespace Purchasing.Web.Services
{
    //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/suggestions.json?category=31579&sort=newest";
    //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/categories/31577.json";
    //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/categories.json";
    //const string query = "https://ucdavis.uservoice.com/api/v1/users/24484752.json";

    public interface IUservoiceService
    {
        int GetActiveIssuesCount();

        /// <summary>
        /// Returns the open issues in uservoice
        /// </summary>
        JObject GetOpenIssues();

        /// <summary>
        /// Filters a list of issues by status name (can be null)
        /// </summary>
        List<JToken> FilterIssuesByStatus(List<JToken> issues, string status);

        /// <summary>
        /// Set the status of any issue
        /// </summary>
        /// <param name="id">issue id</param>
        /// <param name="status">Must be one of the 5 status options on ucdavis.uservoice</param>
        /// <param name="statusUpdateNote">Optional note to be associated with the status update</param>
        /// <param name="notify">true if anyone associated with the issue should be notified of the status update</param>
        void SetIssueStatus(int id, string status, string statusUpdateNote, bool notify = false);
    }

    /// <summary>
    /// Encapsulates API calls to uservoice using OAuth
    /// </summary>
    /// <remarks>See docs for result shape and call options</remarks>
    /// <see cref="http://developer.uservoice.com/docs/api-public/"/>
    public class UservoiceService : IUservoiceService
    {
        private static readonly string ApiKey = WebConfigurationManager.AppSettings["uservoiceKey"];
        private static readonly string ApiSecret = WebConfigurationManager.AppSettings["uservoiceSecret"];
        private static readonly string TokenKey = WebConfigurationManager.AppSettings["uservoiceToken"];
        private static readonly string TokenSecret = WebConfigurationManager.AppSettings["uservoiceTokenSecret"];
        private const string ApiUrlBase = "https://ucdavis.uservoice.com";
        private const string ForumId = "126891";
        private const string IssuesCategoryId = "31579";

        /// <summary>
        /// Returns the open issues in uservoice
        /// </summary>
        public JObject GetOpenIssues()
        {
            string endpoint = CreateEndpoint("/api/v1/forums/{0}/suggestions.json?category={1}&filter=public&sort=newest&per_page=100");

            var result = PerformApiCall(endpoint);

            return JObject.Parse(result);
        }
        
        /// <summary>
        /// Set the status of any issue
        /// </summary>
        /// <param name="id">issue id</param>
        /// <param name="status">Must be one of the 5 status options on ucdavis.uservoice</param>
        /// <param name="statusUpdateNote">Optional note to be associated with the status update</param>
        /// <param name="notify">true if anyone associated with the issue should be notified of the status update</param>
        public void SetIssueStatus(int id, string status, string statusUpdateNote, bool notify = false)
        {
            var parameters = string.Format("notify={0}&response[status]={1}",
                                           notify.ToString(CultureInfo.InvariantCulture).ToLower(), status);

            if (!string.IsNullOrWhiteSpace(statusUpdateNote))
            {
                parameters += string.Format("&response[text]={0}", statusUpdateNote);
            }

            string endpoint = string.Format("/api/v1/forums/{0}/suggestions/{1}/respond.json?{2}", ForumId, id, parameters);

            PerformApiCall(endpoint, "PUT");
        }

        public int GetActiveIssuesCount()
        {
            string endpoint = CreateEndpoint("/api/v1/forums/{0}/categories.json");

            var result = JObject.Parse(PerformApiCall(endpoint));

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
        private string PerformApiCall(string endpoint, string method = "GET")
        {
            var query = ApiUrlBase + endpoint;

            var oauth = new Manager();
            oauth["consumer_key"] = ApiKey;
            oauth["consumer_secret"] = ApiSecret;
            oauth["token"] = TokenKey;
            oauth["token_secret"] = TokenSecret;
        
            var header = oauth.GenerateAuthzHeader(query, method);

            var req = (HttpWebRequest)WebRequest.Create(query);
            req.Method = method;
            req.ContentLength = 0; //No body
            req.Headers.Add("Authorization", header);

            using (var response = (HttpWebResponse) req.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Filters a list of issues by status name (can be null)
        /// </summary>
        public List<JToken> FilterIssuesByStatus(List<JToken> issues, string status)
        {
            return issues.Where(x => x["status"].Value<string>() == status).ToList();
        }
        
        public struct Status
        {
            public static readonly string UnderReview = "under review";
            public static readonly string Planned = "planned";
            public static readonly string Started = "started";
            public static readonly string Completed = "completed";
            public static readonly string Denied = "denied";
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