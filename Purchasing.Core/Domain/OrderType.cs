using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderType : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
    }

    public class OrderTypeMap : ClassMap<OrderType>
    {
        public OrderTypeMap()
        {
            Map(x => x.Name);
        }
    }
}