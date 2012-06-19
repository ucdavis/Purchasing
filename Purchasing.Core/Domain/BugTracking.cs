using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class BugTracking : DomainObject
    {
        public BugTracking()
        {
            DateTimeStamp = DateTime.Now;
        }

        public virtual int OrderId { get; set; }
        [Required]
        [StringLength(20)]
        public virtual string UserId { get; set; }
        public virtual DateTime DateTimeStamp { get; set; }
        [StringLength(500)]
        public virtual string TrackingMessage { get; set; }
        public virtual int? SplitId { get; set; }
        public virtual int? LineItemId { get; set; }

    }

    public class BugTrackingMap : ClassMap<BugTracking>
    {
        public BugTrackingMap()
        {
            Table("BugTracking");

            Id(x => x.Id);

            Map(x => x.OrderId);
            Map(x => x.UserId);
            Map(x => x.DateTimeStamp);
            Map(x => x.TrackingMessage);
            Map(x => x.SplitId);
            Map(x => x.LineItemId);
        }
    }
}
