using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Notification : DomainObjectWithTypedId<Guid>
    {
        [Required]
        public virtual User User { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime? Sent { get; set; }
        public virtual string Status { get; set; }
        public virtual bool Pending { get; set; }
        public virtual bool PerEvent { get; set; }
        public virtual bool Daily { get; set; }
        public virtual bool Weekly { get; set; }
    }

    public class NotificationMap : ClassMap<Notification>
    {
        public NotificationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Created);
            Map(x => x.Sent).ReadOnly();
            Map(x => x.Status).ReadOnly();
            Map(x => x.Pending);
            Map(x => x.PerEvent);
            Map(x => x.Daily);
            Map(x => x.Weekly);

            References(x => x.User);
        }
    }
}
