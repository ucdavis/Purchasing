using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using Dapper;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.ServiceRuntime;
using Purchasing.Core.Queries;
using Purchasing.Web.Services;

namespace Purchasing.Web.App_Start
{
    /*
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
             
            var indexService = new IndexService(new DbService());
            
            while (true)
            {
                try
                {
                    indexService.CreateHistoricalOrderIndex();
                }
                catch (Exception ex)
                {
                    new System.Net.Mail.SmtpClient("smtp.ucdavis.edu").Send("srkirkland@ucdavis.edu", "srkirkland@ucdavis.edu", "error", ex.ToString());
                }
                Thread.Sleep(TimeSpan.FromMinutes(15));
            }
        }
    }
     */
}