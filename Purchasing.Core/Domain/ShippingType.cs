using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ShippingType : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual string Warning { get; set; }
    }

    public class ShippingTypeMap : ClassMap<ShippingType>
    {
        public ShippingTypeMap()
        {
            Map(x => x.Name);
            Map(x => x.Warning);
        }
    }
}