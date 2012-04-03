using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorkgroupIndexModel
    {
        public IEnumerable<Workgroup> WorkGroups { get; set; }
        public bool ShowAll { get; set; }

    }
}