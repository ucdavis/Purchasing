using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class CustomField : DomainObject
    {
        public CustomField()
        {
            Rank = 0;
            IsActive = true;
            IsRequired = false;
        }

        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual Organization Organization { get; set; }
        public virtual int Rank { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsRequired { get; set; }
    }

    public class CustomFieldMap : ClassMap<CustomField>
    {
        public CustomFieldMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Length(int.MaxValue);
            References(x => x.Organization);
            Map(x => x.Rank);
            Map(x => x.IsActive);
            Map(x => x.IsRequired);
        }
    }
}
