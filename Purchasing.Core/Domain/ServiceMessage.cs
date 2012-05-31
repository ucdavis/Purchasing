using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ServiceMessage : DomainObject
    {
        public ServiceMessage()
        {
            BeginDisplayDate = DateTime.Now;
            Critical = false;
            IsActive = true;
        }

        [Required]
        public virtual string Message { get; set; }
        public virtual DateTime BeginDisplayDate { get; set; }
        public virtual DateTime? EndDisplayDate { get; set; }
        public virtual bool Critical { get; set; }
        public virtual bool IsActive { get; set; }
    }

    public class ServiceMessageMap : ClassMap<ServiceMessage>
    {
        public ServiceMessageMap()
        {
            Id(x => x.Id);

            Map(x => x.Message).Length(int.MaxValue);
            Map(x => x.BeginDisplayDate);
            Map(x => x.EndDisplayDate);
            Map(x => x.Critical);
            Map(x => x.IsActive);
        }
    }
}
