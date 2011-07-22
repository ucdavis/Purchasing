using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ApprovalType : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
    }

    public class ApprovalTypeMap : ClassMap<ApprovalType>
    {
        public ApprovalTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
        }
    }
}