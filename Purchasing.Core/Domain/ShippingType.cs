using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ShippingType : DomainObjectWithTypedId<string>
    {
        public ShippingType()
        {
            
        }

        public ShippingType(string id) : this() { Id = id; }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        public virtual string Warning { get; set; }
    }

    public class ShippingTypeMap : ClassMap<ShippingType>
    {
        public ShippingTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.Warning);
        }
    }
}