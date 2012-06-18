using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class KfsDocument : DomainObject
    {
        [Required]
        [StringLength(50)]
        public virtual string DocNumber { get; set; }

        [Required]
        public virtual Order Order { get; set; }
    }

    public class KfsDocumentMap : ClassMap<KfsDocument>
    {
        public KfsDocumentMap()
        {
            Id(x => x.Id);

            Map(x => x.DocNumber);

            References(x => x.Order);
        }
    }
}
