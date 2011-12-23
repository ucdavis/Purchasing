using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class CustomFieldAnswer : DomainObject
    {
        [Required]
        public virtual string Answer { get; set; }
        [Required]
        public virtual CustomField CustomField { get; set; }
        [Required]
        public virtual Order Order { get; set; }
    }

    public class CustomFieldAnswerMap : ClassMap<CustomFieldAnswer>
    {
        public CustomFieldAnswerMap()
        {
            Id(x => x.Id);

            Map(x => x.Answer);
            References(x => x.CustomField);
            References(x => x.Order);
        }
    }
}
