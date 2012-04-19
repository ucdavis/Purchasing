using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderRequestSave : DomainObjectWithTypedId<Guid>
    {
        public OrderRequestSave()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            DateCreated = DateTime.Now;
            LastUpdate = DateTime.Now;

            Attachments = new List<Attachment>();
        }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        public virtual string Comments { get; set; }

        [Required]
        public virtual User User { get; set; }
        public virtual User PreparedBy { get; set; }
        [Required]
        public virtual string FormData { get; set; }
        [Required]
        public virtual string AccountData { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime LastUpdate { get; set; }
        [Required]
        [StringLength(15)]
        public virtual string Version { get; set; }

        public virtual IList<Attachment> Attachments { get; set; }
    }

    public class OrderRequestSaveMap : ClassMap<OrderRequestSave>
    {
        public OrderRequestSaveMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            References(x => x.User);
            //References(x => x.PreparedBy);

            Map(x => x.Name);
            Map(x => x.Comments);

            Map(x => x.FormData);
            Map(x => x.AccountData);
            Map(x => x.DateCreated);
            Map(x => x.LastUpdate);
            Map(x => x.Version);

            HasManyToMany(x => x.Attachments).Table("OrderRequestSavesXAttachments").ParentKeyColumn("OrderRequestSaveId").ChildKeyColumn("AttachmentId");
        }
    }
}
