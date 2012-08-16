using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ControlledSubstanceInformation : DomainObject
    {
        [StringLength(10)]
        [Required]
        public virtual string ClassSchedule { get; set; }
        [StringLength(200)]
        [Required]
        public virtual string Use { get; set; }
        [StringLength(50)]
        [Required]
        public virtual string StorageSite { get; set; }
        [StringLength(200)]
        [Required]
        public virtual string Custodian { get; set; }
        [StringLength(200)]
        [Required]
        public virtual string EndUser { get; set; }
        [Required]
        public virtual Order Order { get; set; }

        public virtual bool PharmaceuticalGrade { get; set; }
    }

    public class ControlledSubstanceInformationMap : ClassMap<ControlledSubstanceInformation>
    {
        public ControlledSubstanceInformationMap()
        {
            Table("ControlledSubstanceInformation");

            Id(x => x.Id);

            Map(x => x.ClassSchedule);
            Map(x => x.Use).Column("`Use`");
            Map(x => x.StorageSite);
            Map(x => x.Custodian);
            Map(x => x.EndUser);
            Map(x => x.PharmaceuticalGrade);

            References(x => x.Order);
        }
    }

}
