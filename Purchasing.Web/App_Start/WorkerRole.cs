using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Purchasing.Web.App_Start
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(15));


            }
        }
    }
}