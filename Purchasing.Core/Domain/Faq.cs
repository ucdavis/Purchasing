using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Faq : DomainObject
    {
        [Required]
        public virtual FaqCategory Category { get; set; }
        [Required]
        public virtual string Question { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public virtual string Answer { get; set; }
        [StringLength(10)]
        public virtual string OrgId { get; set; }
        public virtual int Like { get; set; }
    }

    public enum FaqCategory
    {
        General,
        Requester,
        Approver,
        ConditionalApprover,
        AccountManager,
        Purchaser
    }

    public class FaqMap : ClassMap<Faq>
    {
        public FaqMap()
        {
            Id(x => x.Id);

            Map(x => x.Category).Column("`Category`").CustomType(typeof(NHibernate.Type.EnumStringType<FaqCategory>)).Not.Nullable();
            Map(x => x.Question);
            Map(x => x.Answer);
            Map(x => x.OrgId);
            Map(x => x.Like).Column("`Like`");
        }
    }
}
