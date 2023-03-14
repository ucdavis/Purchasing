using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchasing.Core.Models.Configuration
{
    public class AggieEnterpriseOptions
    {
        public string GraphQlUrl { get; set; }
        public string Token { get; set; }

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string TokenEndpoint { get; set; }
        public string ScopeApp { get; set; }
        public string ScopeEnv { get; set; }

        public string RequisitionSourceName { get; set; }

        public string DefaultNaturalAccount { get; set; }

    }
}
