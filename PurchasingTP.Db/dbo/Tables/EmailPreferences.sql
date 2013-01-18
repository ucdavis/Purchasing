CREATE TABLE [dbo].[EmailPreferences] (
    [Id]                                  VARCHAR (10) NOT NULL,
    [RequesterOrderSubmission]            BIT          NOT NULL,
    [RequesterApproverApproved]           BIT          NOT NULL,
    [RequesterApproverChanged]            BIT          NOT NULL,
    [RequesterAccountManagerApproved]     BIT          NOT NULL,
    [RequesterAccountManagerChanged]      BIT          NOT NULL,
    [RequesterPurchaserAction]            BIT          NOT NULL,
    [RequesterPurchaserChanged]           BIT          NOT NULL,
    [RequesterKualiProcessed]             BIT          NOT NULL,
    [RequesterKualiApproved]              BIT          NOT NULL,
    [RequesterReceived]                   BIT          CONSTRAINT [DF_EmailPreferences_RequesterReceived] DEFAULT ((1)) NOT NULL,
    [ApproverAccountManagerApproved]      BIT          NOT NULL,
    [ApproverAccountManagerDenied]        BIT          NOT NULL,
    [ApproverKualiApproved]               BIT          NOT NULL,
    [ApproverPurchaserProcessed]          BIT          NOT NULL,
    [ApproverOrderCompleted]              BIT          NOT NULL,
    [ApproverOrderArrive]                 BIT          CONSTRAINT [DF_EmailPreferences_ApprovedOrderAssigned] DEFAULT ((1)) NOT NULL,
    [AccountManagerKualiApproved]         BIT          NOT NULL,
    [AccountManagerOrderCompleted]        BIT          NOT NULL,
    [AccountManagerPurchaserProcessed]    BIT          NOT NULL,
    [AccountManagerOrderArrive]           BIT          CONSTRAINT [DF_EmailPreferences_AccountManagerOrderArrive] DEFAULT ((1)) NOT NULL,
    [PurchaserKualiApproved]              BIT          NOT NULL,
    [PurchaserOrderCompleted]             BIT          NOT NULL,
    [PurchaserOrderArrive]                BIT          CONSTRAINT [DF_EmailPreferences_PurchaserOrderArrive] DEFAULT ((1)) NOT NULL,
    [PurchaserKfsItemReceived]            BIT          CONSTRAINT [DF__tmp_ms_xx__Purch__38E6B0D4] DEFAULT ((1)) NOT NULL,
    [PurchaserPCardItemReceived]          BIT          CONSTRAINT [DF__tmp_ms_xx__Purch__39DAD50D] DEFAULT ((1)) NOT NULL,
    [PurchaserCampusServicesItemReceived] BIT          CONSTRAINT [DF__tmp_ms_xx__Purch__3ACEF946] DEFAULT ((1)) NOT NULL,
    [NotificationType]                    VARCHAR (50) NOT NULL,
    [AddAttachment] BIT NOT NULL DEFAULT 1, 
    [AddNote] BIT NOT NULL DEFAULT 1, 
    [ShowAccountInEmail] BIT NOT NULL DEFAULT ((0)), 
    CONSTRAINT [PK_EmailPreferences] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [EmailPreferences_UserId_IDX]
    ON [dbo].[EmailPreferences]([Id] ASC);

