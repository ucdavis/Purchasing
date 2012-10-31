using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Linq;
using Purchasing.WS.AzureStorage;

namespace Purchasing.WS
{
    public class AzureStorageService
    {
        private readonly string _storageUrl;
        private readonly string _serverName;
        private readonly string _username;
        private readonly string _password;
        public string ServiceUrl = @"https://by1prod-dacsvc.azure.com/DACWebService.svc/{0}";    // west coast data center
        public string StatusParameters = @"?servername={0}&username={1}&password={2}&reqId={3}";

        public AzureStorageService(string storageUrl, string serverName, string username, string password)
        {
            _storageUrl = storageUrl;
            _serverName = serverName;
            _username = username;
            _password = password;
        }

        public string Backup(string database, string storageKey)
        {
            var time = DateTime.Now;
            var filename = string.Format("{0}-{1}-{2}-{3}-{4}-{5}.bacpac", database, time.Year, time.Month, time.Day, time.Hour, time.Minute);
            var credentials = new BlobStorageAccessKeyCredentials() { StorageAccessKey = storageKey, Uri = _storageUrl + filename };

            var connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = _serverName;
            connectionInfo.DatabaseName = database;
            connectionInfo.UserName = _username;
            connectionInfo.Password = _password;

            var exportInput = new ExportInput() { ConnectionInfo = connectionInfo, BlobCredentials = credentials };

            // create the request
            var request = WebRequest.Create(string.Format(ServiceUrl, "Export"));
            request.Method = "POST";
            request.ContentType = "application/xml";
            using (var stream = request.GetRequestStream())
            {
                var dcs = new DataContractSerializer(typeof(ExportInput));
                dcs.WriteObject(stream, exportInput);
            }

            // make the post
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpException((int)response.StatusCode, response.StatusDescription);
                }

                var sr = new System.IO.StreamReader(response.GetResponseStream());
                var result = sr.ReadToEnd().Trim();

                var xml = XDocument.Parse(result);
                var node = xml.Descendants().First();
                return node.Value;
            }

            return string.Empty;
        }
        public string GetStatus(string requestId)
        {
            var url = string.Format(ServiceUrl, "Status") + string.Format(StatusParameters, _serverName, _username, _password, requestId);

            var result = XDocument.Load(url);

            var node = result.Descendants().FirstOrDefault(a => a.Name.LocalName == "Status");

            if (node != null)
            {
                return node.Value;
            }

            return string.Empty;
        }
    }
}
