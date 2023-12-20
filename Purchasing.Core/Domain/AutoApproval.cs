using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class AutoApproval : DomainObject, IValidatableObject
    {
        public virtual User TargetUser { get; set; }
        public virtual Account Account { get; set; }
        public virtual decimal MaxAmount { get; set; }
        public virtual bool LessThan { get; set; }
        public virtual bool Equal { get; set; }
        public virtual bool IsActive { get; set; }
        [Required]
        public virtual User User { get; set; }  //Approver who created the rule

        [DataType(DataType.DateTime)]
        public virtual DateTime? Expiration { get; set; }


        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if((TargetUser == null && Account == null) || (TargetUser != null && Account != null))
            {
                //yield return new ValidationResult("An account OR user must be selected, not both.", new[] { "Account" });
                yield return new ValidationResult("A user must be selected.", new[] { "TargetUser" });
            }
        }

    }

    public class AutoApprovalMap : ClassMap<AutoApproval>
    {
        public AutoApprovalMap()
        {
            Id(x => x.Id);

            Map(x => x.MaxAmount);
            Map(x => x.LessThan);
            Map(x => x.Equal);
            Map(x => x.IsActive);
            Map(x => x.Expiration).Nullable();

            References(x => x.TargetUser).Column("TargetUserId").Nullable();
            References(x => x.Account).Nullable();
            References(x => x.User);
        }
    }
}
