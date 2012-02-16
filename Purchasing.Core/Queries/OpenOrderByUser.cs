using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class OpenOrderByUser : DomainObject
    {
        public virtual int OrderId { get; set; }
        public virtual string RequestNumber { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime? DateNeeded { get; set; }
        public virtual string Creator { get; set; }
        public virtual DateTime LastActionDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string Summary { get; set; }
        public virtual string UserAccessId { get; set; }
    }

    public class OpenOrderByUserMap : ClassMap<OpenOrderByUser>
    {
        public OpenOrderByUserMap()
        {
            Id(x => x.Id);

            Table("vOpenOrdersByUser");
            ReadOnly();

            Map(x => x.OrderId);
            Map(x => x.RequestNumber);
            Map(x => x.DateCreated);
            Map(x => x.DateNeeded);
            Map(x => x.Creator);
            Map(x => x.LastActionDate);
            Map(x => x.Name);
            Map(x => x.Summary);
            Map(x => x.UserAccessId);
        }
    }
}
