using System;
using Serilog;

namespace Purchasing.Jobs.Common.Logging
{
    public static class LogHelper
    {
        private static bool _loggingSetup = false;

        public static void ConfigureLogging()
        {
            if (_loggingSetup) return; //only setup logging once

            Log.Logger = new LoggerConfiguration().WriteTo.Stackify().CreateLogger();

            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) =>
                    Log.Error(eventArgs.ExceptionObject as Exception, eventArgs.ExceptionObject.ToString());

            _loggingSetup = true;
        }
    }
}
