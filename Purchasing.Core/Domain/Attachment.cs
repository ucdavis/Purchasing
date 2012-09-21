using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Attachment : DomainObjectWithTypedId<Guid>
    {
        [Required]
        [StringLength(100)]
        public virtual string FileName { get; set; }
        [Required]
        [StringLength(200)]
        public virtual string ContentType { get; set; }
        [Required]
        public virtual byte[] Contents { get; set; }
        public virtual DateTime DateCreated { get; set; }

        [Required]
        public virtual User User { get; set; }

        public virtual Order Order { get; set; }
        [StringLength(50)]
        public virtual string Category { get; set; }
    }

    public class AttachmentMap : ClassMap<Attachment>
    {
        public AttachmentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.FileName);
            Map(x => x.ContentType);
            Map(x => x.Contents).CustomType("BinaryBlob");
            Map(x => x.DateCreated);
            Map(x => x.Category);

            References(x => x.User);
            References(x => x.Order);
        }
    }
}