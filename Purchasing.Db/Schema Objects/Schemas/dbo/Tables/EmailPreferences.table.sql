CREATE TABLE [dbo].[EmailPreferences] (
    [Id]                               VARCHAR (10) NOT NULL,
    [RequesterOrderSubmission]         BIT          NOT NULL,
    [RequesterApproverApproved]        BIT          NOT NULL,
    [RequesterApproverChanged]         BIT          NOT NULL,
    [RequesterAccountManagerApproved]  BIT          NOT NULL,
    [RequesterAccountManagerChanged]   BIT          NOT NULL,
    [RequesterPurchaserAction]         BIT          NOT NULL,
    [RequesterPurchaserChanged]        BIT          NOT NULL,
    [RequesterKualiProcessed]          BIT          NOT NULL,
    [RequesterKualiApproved]           BIT          NOT NULL,
	[RequesterReceived]					BIT			NOT NULL DEFAULT ((1)),
    [ApproverAccountManagerApproved]   BIT          NOT NULL,
    [ApproverAccountManagerDenied]     BIT          NOT NULL,
    [ApproverKualiApproved]            BIT          NOT NULL,
    [ApproverPurchaserProcessed]       BIT          NOT NULL,
    [ApproverOrderCompleted]           BIT          NOT NULL,
    [ApproverOrderArrive]              BIT          NOT NULL,
    [AccountManagerKualiApproved]      BIT          NOT NULL,
    [AccountManagerOrderCompleted]     BIT          NOT NULL,
    [AccountManagerPurchaserProcessed] BIT          NOT NULL,
    [AccountManagerOrderArrive]        BIT          NOT NULL,
    [PurchaserKualiApproved]           BIT          NOT NULL,
    [PurchaserOrderCompleted]          BIT          NOT NULL,
    [PurchaserOrderArrive]             BIT          NOT NULL,
	[PurchaserKfsItemReceived]			BIT NOT NULL DEFAULT 1,
	[PurchaserPCardItemReceived]			BIT NOT NULL DEFAULT 1,
	[PurchaserCampusServicesItemReceived]			BIT NOT NULL DEFAULT 1,
    [NotificationType]                 VARCHAR (50) NOT NULL
);









