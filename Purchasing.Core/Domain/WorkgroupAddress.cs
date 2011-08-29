using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupAddress : DomainObject
    {
        public virtual Workgroup Workgroup { get; set; }
        public virtual string Name { get; set; }
        public virtual string Building { get; set; }
        public virtual string Room { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Phone { get; set; }
    }

    public class WorkgroupAddressMap : ClassMap<WorkgroupAddress>
    {
        public WorkgroupAddressMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.Building);
            Map(x => x.Room);
            Map(x => x.Address);
            Map(x => x.City);
            Map(x => x.State).Column("StateId"); //TODO: make reference to state table?
            Map(x => x.Zip);
            Map(x => x.Phone);

            References(x => x.Workgroup);
        }
    }
}