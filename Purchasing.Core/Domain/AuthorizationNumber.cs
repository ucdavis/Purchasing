using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class AuthorizationNumber : DomainObject
    {
        [StringLength(10)]
        [Required]
        public virtual string AuthorizationNum { get; set; }
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
    }

    public class AuthorizationNumberMap : ClassMap<AuthorizationNumber>
    {
        public AuthorizationNumberMap()
        {
            Id(x => x.Id);

            Map(x => x.AuthorizationNum);
            Map(x => x.ClassSchedule);
            Map(x => x.Use);
            Map(x => x.StorageSite);
            Map(x => x.Custodian);
            Map(x => x.EndUser);

            References(x => x.Order);
        }
    }

}
