using FluentNHibernate.Mapping;
using Newtonsoft.Json;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    [JsonObject]
    public class Commodity : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual string GroupCode { get; set; }
        public virtual string SubGroupCode { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string NameAndId {get { return string.Format("{0} ({1})", Name, Id); }}
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
            Map(x => x.IsActive);
        }
    }
}