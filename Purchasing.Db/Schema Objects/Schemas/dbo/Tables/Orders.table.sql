CREATE TABLE [dbo].[Orders] (
    [Id]                      INT           IDENTITY (1, 1) NOT NULL,
    [OrderTypeId]             CHAR (3)      NOT NULL,
    [WorkgroupVendorId]       INT           NULL,
    [WorkgroupAddressId]      INT           NOT NULL,
    [ShippingTypeId]          CHAR (2)      NULL,
    [DateNeeded]              DATETIME2 (7) NOT NULL,
    [AllowBackorder]          BIT           NOT NULL,
    [EstimatedTax]            FLOAT         NULL,
    [WorkgroupId]             INT           NOT NULL,
    [OrganizationId]          VARCHAR (10)  NOT NULL,
    [ReferenceNumber]         VARCHAR (50)  NULL,
    [LastCompletedApprovalId] INT           NULL,
    [ShippingAmount]          MONEY         NULL,
    [FreightAmount]           MONEY         NULL,
    [DeliverTo]               VARCHAR (50)  NOT NULL,
    [DeliverToEmail]          VARCHAR (50)  NULL,
	    [DeliverToPhone] VARCHAR(15) NULL,
    [Justification]           VARCHAR (MAX) NOT NULL,
    [OrderStatusCodeId]       CHAR (2)      NOT NULL,
    [CreatedBy]               VARCHAR (10)  NOT NULL,
    [DateCreated]             DATETIME      NOT NULL,
    [HasAuthorizationNum]     BIT           NOT NULL,
    [Total]                   MONEY         NOT NULL,
    [CompletionReason]        VARCHAR (MAX) NULL,
    [RequestNumber]           VARCHAR (20)  NOT NULL,
	[KfsDocType]			char(3) null
);















































