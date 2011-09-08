using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderType : DomainObjectWithTypedId<string>
    {
        public OrderType()
        {
            
        }

        public OrderType(string id)
        {
            Id = id;
        }

        public virtual string Name { get; set; }
    }

    public class OrderTypeMap : ClassMap<OrderType>
    {
        public OrderTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
        }
    }
}