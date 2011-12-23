using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class CustomField : DomainObject
    {
        public CustomField()
        {
            Order = 0;
            IsActive = true;
            IsRequired = false;
        }

        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual Organization Organization { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsRequired { get; set; }
    }

    public class CustomFieldMap : ClassMap<CustomField>
    {
        public CustomFieldMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            References(x => x.Organization);
            Map(x => x.Order).Column("`Order`");
            Map(x => x.IsActive);
            Map(x => x.IsRequired);
        }
    }
}
