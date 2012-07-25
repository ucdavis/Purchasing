using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupPermission : DomainObject
    {
        public WorkgroupPermission()
        {
            IsAdmin = false;
        }

        [Required]
        public virtual Workgroup Workgroup { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Role Role { get; set; }

        public virtual bool IsAdmin { get; set; }
        /// <summary>
        /// When permission is an admin role, what workgroup is this permission coming from?
        /// </summary>
        public virtual Workgroup ParentWorkgroup { get; set; }
    }

    public class WorkgroupPermissionMap : ClassMap<WorkgroupPermission>
    {
        public WorkgroupPermissionMap()
        {
            Id(x => x.Id);

            References(x => x.Workgroup);
            References(x => x.User);
            References(x => x.Role);

            Map(x => x.IsAdmin);
            References(x => x.ParentWorkgroup).Column("ParentWorkgroupId");
        }
    }
}