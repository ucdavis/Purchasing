using System.Collections.Generic;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Models
{
    public class AccountSelectModel
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
    }
}