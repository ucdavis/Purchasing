using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class UnitOfMeasure : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
    }

    public class UnitOfMeasureMap : ClassMap<UnitOfMeasure>
    {
        public UnitOfMeasureMap()
        {
            ReadOnly();

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
        }
    }
}
