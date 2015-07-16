using System;
using Microsoft.Azure;
using Serilog;

namespace Purchasing.Jobs.Common.Logging
{
    public static class LogHelper
    {
        private static readonly string ElmahLogId = CloudConfigurationManager.GetSetting("ElmahJobsLogId");
        private static bool _loggingSetup = false;
        public static void ConfigureLogging()
        {
            if (_loggingSetup) return; //only setup logging once

            Guid logId;
            if (Guid.TryParse(ElmahLogId, out logId))
            {
                Log.Logger = new LoggerConfiguration().WriteTo.ElmahIO(logId).CreateLogger();

                AppDomain.CurrentDomain.UnhandledException +=
                    (sender, eventArgs) =>
                        Log.Error(eventArgs.ExceptionObject as Exception, eventArgs.ExceptionObject.ToString());

                _loggingSetup = true;
            }
        }
    }
}
