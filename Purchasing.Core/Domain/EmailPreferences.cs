using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class EmailPreferences : DomainObjectWithTypedId<string>
    {
        public EmailPreferences() { }
        public EmailPreferences(string id) { Id = id; }

        public virtual bool RequesterOrderSubmission { get; set; }

        public virtual bool RequesterApproverApproved { get; set; }
        public virtual bool RequesterApproverChanged { get; set; }

        public virtual bool RequesterAccountManagerApproved { get; set; }
        public virtual bool RequesterAccountManagerChanged { get; set; }

        public virtual bool RequesterPurchaserAction { get; set; }
        public virtual bool RequesterPurchaserChanged { get; set; }

        public virtual bool RequesterKualiProcessed { get; set; }
        public virtual bool RequesterKualiApproved { get; set; }

        public virtual bool ApproverAccountManagerApproved { get; set; }
        public virtual bool ApproverAccountManagerDenied { get; set; }
        public virtual bool ApproverPurchaserProcessed { get; set; }
        public virtual bool ApproverKualiApproved { get; set; }
        public virtual bool ApproverOrderCompleted { get; set; }

        public virtual bool AccountManagerPurchaserProcessed { get; set; }
        public virtual bool AccountManagerKualiApproved { get; set; }
        public virtual bool AccountManagerOrderCompleted { get; set; }

        public virtual bool PurchaserKualiApproved { get; set; }
        public virtual bool PurchaserOrderCompleted { get; set; }

        public virtual NotificationTypes NotificationType { get; set; }

        public enum NotificationTypes
        {
            PerEvent,
            Daily,
            Weekly
        }
    }

    public class EmailPreferencesMap : ClassMap<EmailPreferences>
    {
        public EmailPreferencesMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.RequesterOrderSubmission);
            Map(x => x.RequesterApproverApproved);
            Map(x => x.RequesterApproverChanged);
            Map(x => x.RequesterAccountManagerApproved);
            Map(x => x.RequesterAccountManagerChanged);
            Map(x => x.RequesterPurchaserAction);
            Map(x => x.RequesterPurchaserChanged);
            Map(x => x.RequesterKualiProcessed); 
            Map(x => x.RequesterKualiApproved);

            Map(x => x.ApproverAccountManagerApproved);
            Map(x => x.ApproverAccountManagerDenied);
            Map(x => x.ApproverKualiApproved);
            Map(x => x.ApproverPurchaserProcessed);
            Map(x => x.ApproverOrderCompleted);

            Map(x => x.AccountManagerKualiApproved);
            Map(x => x.AccountManagerOrderCompleted);
            Map(x => x.AccountManagerPurchaserProcessed);

            Map(x => x.PurchaserKualiApproved);
            Map(x => x.PurchaserOrderCompleted);

            Map(x => x.NotificationType).CustomType<NHibernate.Type.EnumStringType<EmailPreferences.NotificationTypes>>()
                .Not.Nullable();
        }
    }
}