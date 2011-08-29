using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Commodity : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual string GroupCode { get; set; }
        public virtual string SubGroupCode { get; set; }
    }

    public class CommodityMap : ClassMap<Commodity>
    {
        public CommodityMap()
        {
            ReadOnly();

            Table("vCommodities");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.GroupCode);
            Map(x => x.SubGroupCode);
        }
    }
}