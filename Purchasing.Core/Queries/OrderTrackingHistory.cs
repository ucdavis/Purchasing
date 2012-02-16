using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class OrderTrackingHistory : DomainObject
    {
        public virtual DateTime TrackingDate { get; set; }
        public virtual int OrderId { get; set; }
        public virtual string RequestNumber { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string AccessUserId { get; set; }
        public virtual string Summary { get; set; }

        /// <summary>
        /// Gets the summary "shortened" with display logic
        /// </summary>
        /// <returns></returns>
        public virtual string DisplaySummary()
        {
            return Summary.Length < 100 ? Summary : string.Format("{0}...", Summary.Substring(0, 100));
        }
    }

    public class OrderTrackingHistoryMap : ClassMap<OrderTrackingHistory>
    {
        public OrderTrackingHistoryMap()
        {
            Id(x => x.Id);

            Table("vOrderTracking");
            ReadOnly();

            Map(x => x.TrackingDate);
            Map(x => x.OrderId);
            Map(x => x.RequestNumber);
            Map(x => x.DateCreated);
            Map(x => x.CreatedBy);
            Map(x => x.AccessUserId);
            Map(x => x.Summary);
        }
    }
}
