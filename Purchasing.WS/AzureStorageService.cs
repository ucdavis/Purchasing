using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Purchasing.WS.AzureStorage;

namespace Purchasing.WS
{
    public class AzureStorageService
    {
        private readonly string _storageUrl;
        private readonly string _serverName;
        private readonly string _sqlUsername;
        private readonly string _sqlPassword;
        private readonly string _storageAccountName;
        private readonly string _storageKey;

        private readonly int _cleanupThreshold;

        private const string ServiceUrl = @"https://by1prod-dacsvc.azure.com/DACWebService.svc/{0}";    // west coast data center
        private const string StatusParameters = @"?servername={0}&username={1}&password={2}&reqId={3}";
        private const string CloudStorageconnectionString = @"DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="sqlUsername"></param>
        /// <param name="sqlPassword"></param>
        /// <param name="storageAccountName"></param>
        /// <param name="storageKey"></param>
        /// <param name="storageUrl">Overrides using the standard domain of blob.core.windows.net, based on storage account name</param>
        /// <param name="cleanupThreshold"># of days to before clearing out old backups</param>
        public AzureStorageService(string serverName, string sqlUsername, string sqlPassword, string storageAccountName, string storageKey, string storageUrl = null, int cleanupThreshold = -4)
        {
            _serverName = serverName;
            _sqlUsername = sqlUsername;
            _sqlPassword = sqlPassword;
            _storageAccountName = storageAccountName;
            _storageKey = storageKey;

            _storageUrl = storageUrl;
            _cleanupThreshold = cleanupThreshold < 0 ? cleanupThreshold : (cleanupThreshold * -1);
        }

        public string StorageUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_storageUrl)) return _storageUrl;

                return string.Format(@"https://{0}.blob.core.windows.net/{1}/", _storageAccountName, _storageKey);
            }
        }

        /// <summary>
        /// Execute the database backup
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public string Backup(string database, out string filename)
        {
            var time = DateTime.Now;
            filename = string.Format("{0}-{1}-{2}-{3}-{4}-{5}.bacpac", database, time.Year, time.Month, time.Day, time.Hour, time.Minute);
            var credentials = new BlobStorageAccessKeyCredentials() { StorageAccessKey = _storageKey, Uri = StorageUrl + filename };

            var connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = _serverName;
            connectionInfo.DatabaseName = database;
            connectionInfo.UserName = _sqlUsername;
            connectionInfo.Password = _sqlPassword;

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
        }

        /// <summary>
        /// Get status of database backup job
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public string GetStatus(string requestId)
        {
            var url = string.Format(ServiceUrl, "Status") + string.Format(StatusParameters, _serverName, _sqlUsername, _sqlPassword, requestId);

            var result = XDocument.Load(url);

            var node = result.Descendants().FirstOrDefault(a => a.Name.LocalName == "Status");

            if (node != null)
            {
                return node.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Cleans up the container, deleting old backupss
        /// </summary>
        /// <remarks>
        /// Code based on example:
        /// https://www.windowsazure.com/en-us/develop/net/how-to-guides/blob-storage/
        /// </remarks>
        /// <param name="containerName">Storage container name</param>
        public IEnumerable<string> BlobCleanup(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(string.Format(CloudStorageconnectionString, _storageAccountName, _storageKey));
            var client = storageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference(containerName);

            var blobs = container.ListBlobs(null, true);
            var filtered = blobs.Where(a => a is CloudBlockBlob && ((CloudBlockBlob)a).Properties.LastModified < DateTime.Now.AddDays(_cleanupThreshold)).ToList();

            var deleted = new List<string>();

            foreach (var item in filtered)
            {
                var blob = (CloudBlockBlob)item;
                deleted.Add(blob.Name);

                blob.Delete();
            }

            return deleted;
        }
    }
}
