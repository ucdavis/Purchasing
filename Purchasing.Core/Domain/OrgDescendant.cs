using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    /// <summary>
    /// Note, this table was originally a readonly one that was access from OrginizationDescendant.cs in the queries folder.
    /// </summary>
    public class OrgDescendant : DomainObject
    {
        public OrgDescendant() 
        {
            IsActive = true;
        }
        [Required]
        [StringLength(10)]
        public virtual string OrgId { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        public virtual string ImmediateParentId { get; set; }
        [Required]
        [StringLength(10)]
        public virtual string RollupParentId { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class OrgDescendantMap : ClassMap<OrgDescendant>
    {
        public OrgDescendantMap()
        {
            Id(x => x.Id);

            Table("vOrganizationDescendants");


            Map(x => x.OrgId);
            Map(x => x.Name);
            Map(x => x.ImmediateParentId);
            Map(x => x.RollupParentId);
            Map(x => x.IsActive);
        }
    }
}
