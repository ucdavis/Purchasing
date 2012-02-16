using System;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class OrderHistoryBase : DomainObject
    {
        public virtual int OrderId { get; set; }
        public virtual string RequestNumber { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateNeeded { get; set; }
        public virtual string Creator { get; set; }
        public virtual DateTime LastActionDate { get; set; }
        public virtual string StatusName { get; set; }
        public virtual string Summary { get; set; }
        public virtual string AccessUserId { get; set; }

        public virtual TimeSpan TimeUntilDue()
        {
            return DateNeeded.HasValue ? DateNeeded.Value - DateTime.Now : TimeSpan.MaxValue;
        }
    }
}