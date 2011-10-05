using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class State : DomainObjectWithTypedId<string>
    {
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

    }

    public class StateMap : ClassMap<State>
    {
        public StateMap()
        {
            ReadOnly();

            Id(a => a.Id).GeneratedBy.Assigned();

            Map(a => a.Name);
        }
    }
}
