CREATE TABLE [dbo].[Orders] (
    [Id]                      INT           IDENTITY (1, 1) NOT NULL,
    [OrderTypeId]             CHAR (3)      NOT NULL,
    [VendorId]                INT           NOT NULL,
    [AddressId]               INT           NOT NULL,
    [ShippingTypeId]          CHAR (2)      NULL,
    [DateNeeded]              DATETIME2 (7) NULL,
    [AllowBackorder]          BIT           NOT NULL,
    [EstimatedTax]            FLOAT         NULL,
    [WorkgroupId]             INT           NOT NULL,
    [OrganizationId]          CHAR (4)      NOT NULL,
    [PONumber]                VARCHAR (50)  NULL,
    [LastCompletedApprovalId] INT           NULL,
    [ShippingAmount]          MONEY         NULL,
    [Justification]           VARCHAR (MAX) NULL,
    [OrderStatusCodeId]       CHAR (2)      NOT NULL,
    [CreatedBy]               VARCHAR (10)  NOT NULL,
    [DateCreated]             DATETIME      NOT NULL
);



















