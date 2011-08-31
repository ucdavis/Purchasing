using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderStatusCode : DomainObjectWithTypedId<string>
    {
        [StringLength(50)]
        [Required]
        public virtual string Name { get; set; }

        public virtual int? Level { get; set; }
        public virtual bool IsComplete { get; set; }
        public virtual bool KfsStatus { get; set; }
    }

    public class OrderStatusCodeMap : ClassMap<OrderStatusCode>
    {
        public OrderStatusCodeMap()
        {
            ReadOnly();
            
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.Level);
            Map(x => x.IsComplete);
            Map(x => x.KfsStatus);
        }
    }
}
