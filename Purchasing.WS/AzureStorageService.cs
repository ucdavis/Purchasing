using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
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
        private readonly string _storageContainer;

        private readonly string _masterDbConnectionString;
        private readonly string _tmpDbConnectionString;

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
        /// <param name="storageContainer"> </param>
        /// <param name="masterDbConnectionString"></param>
        /// <param name="tmpDbConnectionString"></param>
        /// <param name="storageUrl">Overrides using the standard domain of blob.core.windows.net, based on storage account name</param>
        /// <param name="cleanupThreshold"># of days to before clearing out old backups</param>
        public AzureStorageService(string serverName, string sqlUsername, string sqlPassword, string storageAccountName, string storageKey, string storageContainer, string masterDbConnectionString, string tmpDbConnectionString, string storageUrl = null, int cleanupThreshold = -4)
        {
            _serverName = serverName;
            _sqlUsername = sqlUsername;
            _sqlPassword = sqlPassword;
            _storageAccountName = storageAccountName;
            _storageKey = storageKey;
            _storageContainer = storageContainer;

            _masterDbConnectionString = masterDbConnectionString;
            _tmpDbConnectionString = tmpDbConnectionString;

            _storageUrl = storageUrl;
            _cleanupThreshold = cleanupThreshold < 0 ? cleanupThreshold : (cleanupThreshold * -1);
        }

        public string StorageUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_storageUrl)) return _storageUrl;

                return string.Format(@"http://{0}.blob.core.windows.net/{1}/", _storageAccountName, _storageContainer);
            }
        }

        /// <summary>
        /// Execute the database backup
        /// </summary>
        /// <param name="database">Name of database to backup</param>
        /// <param name="tables">List of tables for selective backup, null if backup entire database</param>
        /// <param name="filename">Generated filename for blob storage</param>
        /// <returns></returns>
        public string Backup(string database, List<string> tables, out string filename)
        {
            var time = DateTime.Now;
            filename = string.Format("{0}-{1}-{2}-{3}-{4}-{5}.bacpac", database, time.Year, time.Month, time.Day, time.Hour, time.Minute);
            var credentials = new BlobStorageAccessKeyCredentials() { StorageAccessKey = _storageKey, Uri = StorageUrl + filename };

            var connectionInfo = new ConnectionInfo();
            connectionInfo.ServerName = _serverName;
            connectionInfo.DatabaseName = database;
            connectionInfo.UserName = _sqlUsername;
            connectionInfo.Password = _sqlPassword;

            // create the request
            var request = WebRequest.Create(string.Format(ServiceUrl, tables != null ? "SelectiveExport" : "Export"));
            request.Method = "POST";
            request.ContentType = "application/xml";
            using (var stream = request.GetRequestStream())
            {
                DataContractSerializer dcs;
                if (tables != null)
                {
                    var selectiveExport = new SelectiveExportInput() { ConnectionInfo = connectionInfo, BlobCredentials = credentials, Tables = tables.Select(a => new TableName() { Name = a }).ToArray() };

                    dcs = new DataContractSerializer(typeof(SelectiveExportInput));
                    dcs.WriteObject(stream, selectiveExport);
                }
                else
                {
                    var exportInput = new ExportInput() { ConnectionInfo = connectionInfo, BlobCredentials = credentials };

                    dcs = new DataContractSerializer(typeof(ExportInput));
                    dcs.WriteObject(stream, exportInput);
                }
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
        /// Backup a database that has datasync enabled
        /// </summary>
        /// <param name="database"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string BackupDataSync(string database, out string filename)
        {
            // make a copy that we can alter
            CreateCopy(database);

            // remove the datasync tables/triggers/sprocs
            RemoveDataSync();

            // export the copy to blob storage
            var reqId = Backup(string.Format("{0}Backup", database), null, out filename);

            // keep checking until the export is complete
            do
            {
#if DEBUG
                Console.Clear();
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(reqId);
#endif
                var response = GetStatus(reqId);
#if DEBUG
                Console.WriteLine(GetStatus(reqId));
#endif

                if (response == "Completed") break;

                // wait 30 seconds before checking again
                Thread.Sleep(30000);

            } while (true);

            // drop the temporary database
            CleanupTmp(database);

            return reqId;
        }

        /// <summary>
        /// makes a copy of the database
        /// </summary>
        /// <param name="database"></param>
        private void CreateCopy(string database)
        {
            using (var connection = new SqlConnection(_masterDbConnectionString))
            {
                connection.Open();

                // start the copy job
                var cmd1 = new SqlCommand();
                cmd1.Connection = connection;
                cmd1.CommandText = string.Format("CREATE DATABASE {0}Backup AS COPY OF {0};", database);
                cmd1.ExecuteNonQuery();

                connection.Close();
            }

            // check the status, until it's online
            var status = string.Empty;

            do
            {
                // wait 30 seconds
                Thread.Sleep(30000);

                using (var connection = new SqlConnection(_masterDbConnectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = string.Format("SELECT state_desc FROM sys.databases WHERE name = '{0}Backup'", database);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        status = reader["state_desc"].ToString();
                        Console.WriteLine("{0} as of {1}", status, DateTime.Now);
                    }
                    reader.Close();

                    connection.Close();
                }
            } while (status != "ONLINE");
        }
        /// <summary>
        /// Removes all the necessary tables/sprocs/triggers that are related to DataSync
        /// </summary>
        /// <remarks>
        /// Removing these allows the database to be restored and minimizes a little of the spaced needed for backups.
        /// The triggers are the only necessary part, as they prevent restoration from blob storage.
        /// </remarks>
        /// <param name="database"></param>
        private void RemoveDataSync()
        {
            // drop the triggers
            using (var connection = new SqlConnection(_tmpDbConnectionString))
            {
                var triggers = new List<string>();
                var procedures = new List<string>();
                var tables = new List<string>();
                var ttables = new List<string>();
                var tctables = new List<KeyValuePair<string, string>>();
                var types = new List<string>();

                connection.Open();

                var cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "select * from sysobjects where xtype = 'TR' and name like '%_dss_%'";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        triggers.Add(reader["name"].ToString());
                        Console.WriteLine(reader["name"].ToString());
                    }
                }

                var cmd1a = new SqlCommand();
                cmd1a.Connection = connection;
                cmd1a.CommandText = "select * from sysobjects where id in (select distinct parent_obj from sysobjects where xtype = 'TR' and name like '%_dss_%')";

                using (var reader = cmd1a.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ttables.Add(reader["name"].ToString());
                    }
                }

                var cmd1b = new SqlCommand();
                cmd1b.Connection = connection;
                cmd1b.CommandText = "select objs.name cname, pobjs.name pname from sysobjects objs inner join sysobjects pobjs on objs.parent_obj = pobjs.id where objs.parent_obj in (select distinct parent_obj from sysobjects where xtype = 'TR' and name like '%_dss_%') and objs.xtype = 'F'";

                using (var reader = cmd1b.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tctables.Add(new KeyValuePair<string, string>(reader["cname"].ToString(), reader["pname"].ToString()));
                    }
                }

                var cmd2 = new SqlCommand();
                cmd2.Connection = connection;
                cmd2.CommandText = "select name from sys.procedures where schema_name(schema_id) = 'DataSync'";

                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        procedures.Add("[DataSync].[" + reader["name"].ToString() + "]");
                        Console.WriteLine(reader["name"].ToString());
                    }
                }

                var cmd3 = new SqlCommand();
                cmd3.Connection = connection;
                cmd3.CommandText = "select * from information_schema.tables where table_schema = 'DataSync'";

                using (var reader = cmd3.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add("[DataSync].[" + reader["TABLE_NAME"].ToString() + "]");
                        Console.WriteLine(reader["TABLE_NAME"].ToString());
                    }
                }

                var cmd4 = new SqlCommand("select * from sys.types where is_user_defined = 1 and schema_name(schema_id) = 'DataSync'", connection);
                using (var reader = cmd4.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add("[DataSync].[" + reader["name"] + "]");
                    }
                }

                if (triggers.Count > 0)
                {
                    var dcmd = new SqlCommand(string.Format("drop trigger {0}", string.Join(",", triggers)), connection);
                    dcmd.ExecuteNonQuery();
                }

                if (procedures.Count > 0)
                {
                    var dcmd2 = new SqlCommand(string.Format("drop procedure {0}", string.Join(",", procedures)), connection);
                    dcmd2.ExecuteNonQuery();
                }

                if (tables.Count > 0)
                {
                    var dcmd3 = new SqlCommand(string.Format("drop table {0}", string.Join(",", tables)), connection);
                    dcmd3.ExecuteNonQuery();
                }

                if (tctables.Count > 0)
                {
                    foreach (var constraint in tctables)
                    {
                        var dcmd3b = new SqlCommand(string.Format("alter table {0} drop constraint {1}", constraint.Value, constraint.Key), connection);
                        dcmd3b.ExecuteNonQuery();
                    }
                }

                if (ttables.Count > 0)
                {
                    foreach (var name in ttables)
                    {
                        var dcmd3a = new SqlCommand(string.Format("truncate table {0}", name), connection);
                        dcmd3a.ExecuteNonQuery();
                    }
                }

                if (types.Count > 0)
                {
                    foreach (var type in types)
                    {
                        var dcmd4 = new SqlCommand(string.Format("drop type {0};", type), connection);
                        dcmd4.ExecuteNonQuery();
                    }
                }

                var cmd5 = new SqlCommand("drop schema DataSync", connection);
                cmd5.ExecuteNonQuery();

                connection.Close();
            }
        }
        /// <summary>
        /// Remove the temporary database
        /// </summary>
        /// <param name="database"></param>
        public void CleanupTmp(string database)
        {
            using (var connection = new SqlConnection(_masterDbConnectionString))
            {
                connection.Open();

                // start the copy job
                var cmd1 = new SqlCommand();
                cmd1.Connection = connection;
                cmd1.CommandText = string.Format("DROP DATABASE {0}Backup;", database);
                cmd1.ExecuteNonQuery();

                connection.Close();
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
        public IEnumerable<string> BlobCleanup()
        {
            var storageAccount = CloudStorageAccount.Parse(string.Format(CloudStorageconnectionString, _storageAccountName, _storageKey));
            var client = storageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference(_storageContainer);

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
