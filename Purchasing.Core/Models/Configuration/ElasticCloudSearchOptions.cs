namespace Purchasing.Core.Models.Configuration
{
    public sealed class ElasticCloudSearchOptions
    {
        public string Url { get; set; }
        public string IndexPrefix { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
    }
}
