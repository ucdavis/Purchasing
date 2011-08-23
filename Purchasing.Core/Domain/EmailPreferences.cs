using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class EmailPreferences : DomainObjectWithTypedId<string>
    {
        public virtual bool RequesterOrderSubmission { get; set; }
        public virtual bool RequesterApproverApproved { get; set; }
        public virtual bool RequesterApproverChanged { get; set; }
    }

    public class EmailPreferencesMap : ClassMap<EmailPreferences>
    {
        public EmailPreferencesMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.RequesterOrderSubmission);
            Map(x => x.RequesterApproverApproved);
            Map(x => x.RequesterApproverChanged);
        }
    }
}