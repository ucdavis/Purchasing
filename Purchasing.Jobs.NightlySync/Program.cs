using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;

namespace Purchasing.Jobs.NightlySync
{
    public class Program : WebJobBase
    {
        static void Main(string[] args)
        {
            LogHelper.ConfigureLogging();

            Console.WriteLine("Build Number: {0}", typeof(Program).Assembly.GetName().Version);

            var kernel = ConfigureServices();

            var jobHost = new JobHost();
            jobHost.Call(typeof(Program).GetMethod("NightlySync"));
        }

        [NoAutomaticTrigger]
        public static void NightlySync()
        {

        }
    }
}
