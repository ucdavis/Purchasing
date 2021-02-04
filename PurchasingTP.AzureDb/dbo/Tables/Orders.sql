CREATE TABLE [dbo].[Orders] (
    [Id]                      INT           IDENTITY (1, 1) NOT NULL,
    [OrderTypeId]             CHAR (3)      NOT NULL,
    [WorkgroupVendorId]       INT           NULL,
    [WorkgroupAddressId]      INT           NOT NULL,
    [ShippingTypeId]          CHAR (2)      NULL,
    [DateNeeded]              DATETIME2 (7) NOT NULL,
    [AllowBackorder]          BIT           NOT NULL,
    [EstimatedTax]            FLOAT (53)    NULL,
    [WorkgroupId]             INT           NOT NULL,
    [OrganizationId]          VARCHAR (10)  NOT NULL,
    [ReferenceNumber]         VARCHAR (50)  NULL,
    [LastCompletedApprovalId] INT           NULL,
    [ShippingAmount]          MONEY         NULL,
    [FreightAmount]           MONEY         NULL,
    [DeliverTo]               VARCHAR (50)  NOT NULL,
    [DeliverToEmail]          VARCHAR (50)  NULL,
    [DeliverToPhone]          VARCHAR (15)  NULL,
    [Justification]           VARCHAR (MAX) NOT NULL,
    [BusinessPurpose]         VARCHAR (MAX) NULL,
    [LineItemSummary]         VARCHAR (MAX) NULL,
    [OrderStatusCodeId]       CHAR (2)      NOT NULL,
    [CreatedBy]               VARCHAR (10)  NOT NULL,
    [DateCreated]             DATETIME      NOT NULL,
    [HasAuthorizationNum]     BIT           CONSTRAINT [DF_Orders_HasAuthorizationNum] DEFAULT ((0)) NOT NULL,
    [Total]                   MONEY         CONSTRAINT [DF_Orders_Total] DEFAULT ((0)) NOT NULL,
    [CompletionReason]        VARCHAR (MAX) NULL,
    [RequestNumber]           VARCHAR (20)  NOT NULL,
    [RequestType]             VARCHAR (128) NULL,
    [KfsDocType]              CHAR (3)      NULL,
    [PoNumber]                VARCHAR (50)  NULL,
    [Tag]                     VARCHAR (256) NULL,
    [FpdCompleted]            BIT           DEFAULT ((0)) NOT NULL,
    [ApUser]                  VARCHAR (10)  NULL,
    [PostStatus]              VARCHAR (50)  NULL,
    [OrderNote]               VARCHAR (140) NULL,
    [DateLastAction]          DATETIME      DEFAULT (getutcdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_Orders_OrderStatusCodes] FOREIGN KEY ([OrderStatusCodeId]) REFERENCES [dbo].[OrderStatusCodes] ([Id]),
    CONSTRAINT [FK_Orders_OrderTypes] FOREIGN KEY ([OrderTypeId]) REFERENCES [dbo].[OrderTypes] ([Id]),
    CONSTRAINT [FK_Orders_ShippingTypes] FOREIGN KEY ([ShippingTypeId]) REFERENCES [dbo].[ShippingTypes] ([Id]),
    CONSTRAINT [FK_Orders_WorkgroupAddresses] FOREIGN KEY ([WorkgroupAddressId]) REFERENCES [dbo].[WorkgroupAddresses] ([Id]),
    CONSTRAINT [FK_Orders_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id]),
    CONSTRAINT [FK_Orders_WorkgroupVendors] FOREIGN KEY ([WorkgroupVendorId]) REFERENCES [dbo].[WorkgroupVendors] ([Id])
);


















GO
CREATE NONCLUSTERED INDEX [Orders_WorkgroupVendorId_IDX]
    ON [dbo].[Orders]([WorkgroupVendorId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_workgroupid_IDX]
    ON [dbo].[Orders]([WorkgroupId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_WorkgroupAddressId_IDX]
    ON [dbo].[Orders]([WorkgroupAddressId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_ShippingTypeId_IDX]
    ON [dbo].[Orders]([ShippingTypeId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_OrderTypeId_IDX]
    ON [dbo].[Orders]([OrderTypeId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_orderstatuscodeid_IDX]
    ON [dbo].[Orders]([OrderStatusCodeId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_LastCompletedApprovalId_IDX]
    ON [dbo].[Orders]([LastCompletedApprovalId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_CreatedBy_IDX]
    ON [dbo].[Orders]([CreatedBy] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_OrderStatusCodeId_Incl_IdWorkgroupId_CVIDX]
    ON [dbo].[Orders]([OrderStatusCodeId] ASC)
    INCLUDE([Id], [WorkgroupId]) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Orders_StatusCode_DateLastAction_IDX]
    ON [dbo].[Orders]([OrderStatusCodeId] ASC, [DateLastAction] ASC)
    INCLUDE([WorkgroupId]);

