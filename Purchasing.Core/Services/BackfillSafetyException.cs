using System;

namespace Purchasing.Core.Services
{
    // Only messages authored by the runner, with no SQL/transport exception text or payloads.
    public sealed class BackfillSafetyException : InvalidOperationException
    {
        public BackfillSafetyException(string message) : base(message) { }
    }
}
