using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchasing.Core.Models.AggieEnterprise
{
    public class AccountValidationModel
    {
        public bool IsValid { get; set; } = false;
        public bool IsPpm { get; set; } = false;
        /// <summary>
        /// Return Segment info.
        /// </summary>
        public List<KeyValuePair<string, string>> Details { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Warnings { get; set; } = new List<KeyValuePair<string, string>>();
        public string Message
        {
            get
            {
                if (Messages.Count <= 0)
                {
                    return string.Empty;
                }

                return string.Join(" ", Messages);
            }
        }
        public List<string> Messages { get; set; } = new List<string>();
    }
}
