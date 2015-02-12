using System;
using Purchasing.Core.Helpers;

namespace Purchasing.Core.Queries
{
    public class OrderHistoryBase
    {
        //public virtual int Id { get; set; }
        public virtual int OrderId { get; set; }
        public virtual string RequestNumber { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateNeeded { get; set; }
        public virtual string Creator { get; set; }
        public virtual DateTime LastActionDate { get; set; }
        public virtual string StatusName { get; set; }
        public virtual string Summary { get; set; }
        public virtual string AccessUserId { get; set; }
        public virtual string VendorName { get; set; }
        public virtual bool IsDirectlyAssigned { get; set; }

        public virtual TimeSpan TimeUntilDue()
        {
            return DateNeeded.HasValue ? DateNeeded.Value - DateTime.UtcNow.ToPacificTime() : TimeSpan.MaxValue;
        }

        /// <summary>
        /// Gets the summary "shortened" with display logic
        /// </summary>
        /// <returns></returns>
        public virtual string DisplaySummary()
        {
            return Summary.Length < 100 ? Summary : string.Format("{0}...", Summary.Substring(0, 100));
        }

        public virtual string DisplayVendorOrCreator()
        {
            return !string.IsNullOrEmpty(VendorName) ? VendorName : Creator;
        }
    }
}