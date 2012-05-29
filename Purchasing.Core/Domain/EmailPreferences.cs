using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class EmailPreferences : DomainObjectWithTypedId<string>
    {
        public EmailPreferences()
        {
            RequesterOrderSubmission = true;
            RequesterApproverApproved = true;
            RequesterApproverChanged = true;
            RequesterAccountManagerApproved = true;
            RequesterAccountManagerChanged = true;
            RequesterPurchaserAction = true;
            RequesterPurchaserChanged = true;
            RequesterKualiProcessed = true;
            RequesterKualiApproved = true;

            ApproverAccountManagerApproved = true;
            ApproverAccountManagerDenied = true;
            ApproverPurchaserProcessed = true;
            ApproverKualiApproved = true;
            ApproverOrderCompleted = true;
            ApproverOrderArrive = true;
            
            AccountManagerPurchaserProcessed = true;
            AccountManagerKualiApproved = true;
            AccountManagerOrderCompleted = true;
            AccountManagerOrderArrive = true;

            PurchaserKualiApproved = true;
            PurchaserOrderCompleted = true;
            PurchaserOrderArrive = true;
            PurchaserKfsItemReceived = true;
            PurchaserPCardItemReceived = true;
            PurchaserCampusServicesItemReceived = true;

            NotificationType = NotificationTypes.PerEvent;
        }
        public EmailPreferences(string id) : this() {Id = id;}

        #region Requester Settings

        [Display(Name = "Order Submitted")]
        public virtual bool RequesterOrderSubmission { get; set; }

        [Display(Name="Approver Updates Request")]
        public virtual bool RequesterApproverChanged { get; set; }

        [Display(Name="Approver Reviewed")]
        public virtual bool RequesterApproverApproved { get; set; }

        [Display(Name="Account Manager Updates Request")]
        public virtual bool RequesterAccountManagerChanged { get; set; }

        [Display(Name="Account Manager Reviewed")]
        public virtual bool RequesterAccountManagerApproved { get; set; }

        [Display(Name="Purchaser Updates Request")]
        public virtual bool RequesterPurchaserChanged { get; set; }

        [Display(Name="Purchaser Processed")]
        public virtual bool RequesterPurchaserAction { get; set; }        
        
        [Display(Name="Kuali Updates Request")]
        public virtual bool RequesterKualiProcessed { get; set; }      
  
        [Display(Name="Kuali Approved")]
        public virtual bool RequesterKualiApproved { get; set; }    
    
        #endregion Requester Settings

        #region Approver Settings

        [Display(Name="Account Manager Reviewed")]
        public virtual bool ApproverAccountManagerApproved { get; set; }     
   
        //TODO: Would a PI want to know if the account manager changed anything?

        [Display(Name="Account Manager Denied")]
        public virtual bool ApproverAccountManagerDenied { get; set; }

        [Display(Name="Purchaser Processed")]
        public virtual bool ApproverPurchaserProcessed { get; set; }

        //TODO: Would a PI want to know if the Purchaser changed anything?
        //TODO: Would a PI want to know if the Kuali changed anything?

        [Display(Name="Kuali Approved")]
        public virtual bool ApproverKualiApproved { get; set; }

        [Display(Name="Request Completed")]
        public virtual bool ApproverOrderCompleted { get; set; }

        [Display(Name="Order Arrived")]
        public virtual bool ApproverOrderArrive { get; set; }

        #endregion Approver Settings

        #region Account Manager Settings

        [Display(Name="Purchaser Processed")]
        public virtual bool AccountManagerPurchaserProcessed { get; set; }   

        [Display(Name="Kuali Approved")]
        public virtual bool AccountManagerKualiApproved { get; set; }    

        [Display(Name="Request Completed")]
        public virtual bool AccountManagerOrderCompleted { get; set; } 

        //TODO: Would the account manager want to know if the purchaser changed anything? What if the account was using a specific amount to drain it and that was changed?

        [Display(Name="Order Arrived")]
        public virtual bool AccountManagerOrderArrive { get; set; }

        #endregion Account Manager Settings

        #region Purchaser Settings

        [Display(Name="Kuali Approved")]
        public virtual bool PurchaserKualiApproved { get; set; }

        [Display(Name="Request Completed")]
        public virtual bool PurchaserOrderCompleted { get; set; }   
    
        [Display(Name ="Order Arrived")]
        public virtual bool PurchaserOrderArrive { get; set; }

        [Display(Name = "KFS Item Received")]
        public virtual bool PurchaserKfsItemReceived { get; set; }

        [Display(Name = "P Card Item Received")]
        public virtual bool PurchaserPCardItemReceived { get; set; }

        [Display(Name = "Campus Services Item Received")]
        public virtual bool PurchaserCampusServicesItemReceived { get; set; }

        #endregion Purchaser Settings

        

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
            Map(x => x.ApproverOrderArrive);

            Map(x => x.AccountManagerKualiApproved);
            Map(x => x.AccountManagerOrderCompleted);
            Map(x => x.AccountManagerPurchaserProcessed);
            Map(x => x.AccountManagerOrderArrive);

            Map(x => x.PurchaserKualiApproved);
            Map(x => x.PurchaserOrderCompleted);
            Map(x => x.PurchaserOrderArrive);
            Map(x => x.PurchaserKfsItemReceived);
            Map(x => x.PurchaserPCardItemReceived);
            Map(x => x.PurchaserCampusServicesItemReceived);

            Map(x => x.NotificationType).CustomType<NHibernate.Type.EnumStringType<EmailPreferences.NotificationTypes>>().Not.Nullable();
        }
    }
}