using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class UnitOfMeasure : DomainObjectWithTypedId<string>
    {
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
    }

    public class UnitOfMeasureMap : ClassMap<UnitOfMeasure>
    {
        public UnitOfMeasureMap()
        {
            ReadOnly();
            Cache.ReadOnly().Region("LongTerm");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
        }
    }
}
