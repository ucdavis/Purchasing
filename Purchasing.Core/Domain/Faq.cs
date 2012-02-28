using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Faq : DomainObject
    {
        [StringLength(50)]
        [Required]
        public virtual string Category { get; set; }
        [Required]
        public virtual string Question { get; set; }
        [Required]
        public virtual string Answer { get; set; }
        [StringLength(10)]
        public virtual string OrgId { get; set; }
        public virtual int Like { get; set; }
    }

    public class FaqMap : ClassMap<Faq>
    {
        public FaqMap()
        {
            Id(x => x.Id);

            Map(x => x.Category);
            Map(x => x.Question);
            Map(x => x.Answer);
            Map(x => x.OrgId);
            Map(x => x.Like);
        }
    }
}
