using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class CommodityGroup : DomainObjectWithTypedId<Guid>
    {
        public virtual string GroupCode { get; set; }
        public virtual string Name { get; set; }
        public virtual string SubGroupCode { get; set; }
        public virtual string SubGroupName { get; set; }
    }

    public class CommodityGroupMap : ClassMap<CommodityGroup>
    {
        public CommodityGroupMap()
        {
            ReadOnly();

            Table("vCommodityGroups");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.GroupCode);
            Map(x => x.Name);
            Map(x => x.SubGroupCode);
            Map(x => x.SubGroupName);
        }
    }
}
