using System.ComponentModel;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class EmailPreferences : DomainObjectWithTypedId<string>
    {
        public EmailPreferences() { }
        public EmailPreferences(string id) { Id = id; }

        [DisplayName("Order Submission")]
        public virtual bool RequesterOrderSubmission { get; set; }
         
        [DisplayName("Approver Approved")]
        public virtual bool RequesterApproverApproved { get; set; }
        [DisplayName("Approver Updates Request")]
        public virtual bool RequesterApproverChanged { get; set; }

        [DisplayName("Account Manager Reviewed")]
        public virtual bool RequesterAccountManagerApproved { get; set; }
        [DisplayName("Account Manager Updates Request")]
        public virtual bool RequesterAccountManagerChanged { get; set; }

        [DisplayName("Purchaser Action")]
        public virtual bool RequesterPurchaserAction { get; set; }
        [DisplayName("Purchaser Updates Request")]
        public virtual bool RequesterPurchaserChanged { get; set; }

        [DisplayName("Kuali Update")]
        public virtual bool RequesterKualiProcessed { get; set; }
        [DisplayName("Kuali Approved")]
        public virtual bool RequesterKualiApproved { get; set; }

        [DisplayName("Account Manager Reviewed")]
        public virtual bool ApproverAccountManagerApproved { get; set; }
        [DisplayName("Account MAnager Denied")]
        public virtual bool ApproverAccountManagerDenied { get; set; }
        [DisplayName("Purchaser Processed")]
        public virtual bool ApproverPurchaserProcessed { get; set; }
        [DisplayName("Kuali Approved")]
        public virtual bool ApproverKualiApproved { get; set; }
        [DisplayName("Request Completed")]
        public virtual bool ApproverOrderCompleted { get; set; }

        [DisplayName("Purchaser Processed")]
        public virtual bool AccountManagerPurchaserProcessed { get; set; }
        [DisplayName("Kuali Approved")]
        public virtual bool AccountManagerKualiApproved { get; set; }
        [DisplayName("Request Completed")]
        public virtual bool AccountManagerOrderCompleted { get; set; }

        [DisplayName("Kuali Approved")]
        public virtual bool PurchaserKualiApproved { get; set; }
        [DisplayName("Request Completed")]
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