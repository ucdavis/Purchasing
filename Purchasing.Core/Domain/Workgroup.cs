using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using UCDArch.Core.DomainModel;
using System.Linq;

namespace Purchasing.Core.Domain
{
    public class Workgroup : DomainObject
    {
        public Workgroup()
        {
            Accounts = new List<WorkgroupAccount>();
            Organizations = new List<Organization>();
            Vendors = new List<WorkgroupVendor>();
            Permissions = new List<WorkgroupPermission>();
            Addresses = new List<WorkgroupAddress>();
            ConditionalApprovals = new List<ConditionalApproval>();
            Orders = new List<Order>();
            IsActive = true;
            Administrative = false;
            IsFullFeatured = false;
            SyncAccounts = false;
            RequireApproval = false;
        }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        [Display(Name="Is Administrative")]
        public virtual bool Administrative { get; set; }

        /// <summary>
        /// Denotes if an administrative workgroup is for the Shared Services or Cluster Admins.  Users will receive email and work will be considered primary, rather than administrative.
        /// </summary>
        [Display(Name="Full Featured")]
        public virtual bool IsFullFeatured { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Disclaimer { get; set; }

        [Display(Name="Synchronize Accounts")]
        public virtual bool SyncAccounts { get; set; }

        [Display(Name="Show Controlled Substances Fields")]
        public virtual bool AllowControlledSubstances { get; set; }

        [Display(Name="Force Account Select at Approver")]
        public virtual bool ForceAccountApprover { get; set; }

        public virtual IList<WorkgroupAccount> Accounts { get; set; }
        public virtual IList<Organization> Organizations { get; set; }
        public virtual IList<WorkgroupVendor> Vendors { get; set; }
        public virtual IList<WorkgroupAddress> Addresses { get; set; }
        public virtual IList<WorkgroupPermission> Permissions { get; set; }
        public virtual IList<ConditionalApproval> ConditionalApprovals { get; set; }
        public virtual IList<Order> Orders { get; set; }

        [Required]
        [Display(Name = "Primary Organization")]
        public virtual Organization PrimaryOrganization { get; set; }
        [Display(Name = "Is Active")]
        public virtual bool IsActive { get; set; }

        [StringLength(100)]
        [Email]
        [Display(Name="Notification Email List")]
        public virtual string NotificationEmailList { get; set; }

        [Display(Name = "Require Approval For External Accounts")]
        public virtual bool RequireApproval { get; set; }

        public virtual void AddPermission(WorkgroupPermission workgroupPermission)
        {
            workgroupPermission.Workgroup = this;
            Permissions.Add(workgroupPermission);
        }

        public virtual void AddAccount(WorkgroupAccount workgroupAccount)
        {
            workgroupAccount.Workgroup = this;
            Accounts.Add(workgroupAccount);
        }

        public virtual void AddAddress(WorkgroupAddress workgroupAddress)
        {
            workgroupAddress.Workgroup = this;
            Addresses.Add(workgroupAddress);
        }

        public virtual void AddVendor(WorkgroupVendor workgroupVendor)
        {
            workgroupVendor.Workgroup = this;
            Vendors.Add(workgroupVendor);
        }

        public virtual IList<ConditionalApproval> AllConditionalApprovals
        {
            get { 
                var cas = new List<ConditionalApproval>();

                cas.AddRange(ConditionalApprovals);

                // add in CAs from org(s)
                cas.AddRange(Organizations.SelectMany(a=>a.ConditionalApprovals).ToList());

                return cas.Distinct().ToList();
            }
        }
    }

    public class WorkgroupMap : ClassMap<Workgroup>
    {
        public WorkgroupMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.IsActive);
            Map(x => x.Administrative);
            Map(x => x.IsFullFeatured);
            Map(x => x.Disclaimer).Length(int.MaxValue);
            Map(x => x.SyncAccounts);
            Map(x => x.AllowControlledSubstances);
            Map(x => x.ForceAccountApprover);
            Map(x => x.NotificationEmailList);
            Map(x => x.RequireApproval);

            References(x => x.PrimaryOrganization).Column("PrimaryOrganizationId").Not.Nullable();

            HasMany(x => x.Vendors).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Accounts).ExtraLazyLoad().Cascade.SaveUpdate().Inverse();
            HasMany(x => x.Addresses).ExtraLazyLoad().Cascade.SaveUpdate().Inverse();
            HasMany(x => x.Permissions).ExtraLazyLoad().Cascade.SaveUpdate().Inverse();
            HasMany(x => x.ConditionalApprovals).ExtraLazyLoad().Cascade.SaveUpdate().Inverse();
            HasMany(x => x.Orders).ExtraLazyLoad().Cascade.None().Inverse();

            HasManyToMany(x => x.Organizations).Table("WorkgroupsXOrganizations").ParentKeyColumn("WorkgroupId").
                ChildKeyColumn("OrganizationId").ExtraLazyLoad();
        }
    }
}