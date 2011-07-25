using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Audit : DomainObjectWithTypedId<Guid>
    {
        [StringLength(50)]
        [Required]
        public virtual string ObjectName { get; set; }

        [StringLength(50)]
        public virtual string ObjectId { get; set; }

        [StringLength(1)]
        [Required]
        public virtual string AuditAction { get; set; }

        [StringLength(256)]
        [Required]
        public virtual string Username { get; set; }

        public virtual DateTime AuditDate { get; set; }

        public virtual void SetActionCode(AuditActionType auditActionType)
        {
            switch (auditActionType)
            {
                case AuditActionType.Create:
                    AuditAction = "C";
                    break;
                case AuditActionType.Update:
                    AuditAction = "U";
                    break;
                case AuditActionType.Delete:
                    AuditAction = "D";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("auditActionType");
            }
        }
    }

    public enum AuditActionType
    {
        Create, Update, Delete
    }

    public class AuditMap : ClassMap<Audit>
    {
        public AuditMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.ObjectName);
            Map(x => x.ObjectId);
            Map(x => x.AuditAction);
            Map(x => x.Username);
            Map(x => x.AuditDate);
        }
    }
}