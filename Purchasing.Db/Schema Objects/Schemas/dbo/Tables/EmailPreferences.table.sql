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
    [ApproverAccountManagerApproved]   BIT          NOT NULL,
    [ApproverAccountManagerDenied]     BIT          NOT NULL,
    [ApproverKualiApproved]            BIT          NOT NULL,
    [ApproverPurchaserProcessed]       BIT          NOT NULL,
    [ApproverOrderCompleted]           BIT          NOT NULL,
    [AccountManagerKualiApproved]      BIT          NOT NULL,
    [AccountManagerOrderCompleted]     BIT          NOT NULL,
    [AccountManagerPurchaserProcessed] BIT          NOT NULL,
    [PurchaserKualiApproved]           BIT          NOT NULL,
    [PurchaserOrderCompleted]          BIT          NOT NULL,
    [NotificationType]                 VARCHAR (50) NOT NULL,
    [UserId]                           VARCHAR (10) NOT NULL
);



