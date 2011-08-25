CREATE TABLE [dbo].[Orders] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [OrderTypeId]           CHAR (3)      NOT NULL,
    [VendorId]              INT           NOT NULL,
    [AddressId]             INT           NOT NULL,
    [ShippingTypeId]        CHAR (2)      NULL,
    [DateNeeded]            DATETIME2 (7) NULL,
    [AllowBackorder]        BIT           NOT NULL,
    [EstimatedTax]          FLOAT         NULL,
    [WorkgroupId]           INT           NOT NULL,
    [OrganizationId]        CHAR (4)      NOT NULL,
    [PONumber]              VARCHAR (50)  NULL,
    [LastCompletedApproval] INT           NULL,
    [ShippingAmount]        MONEY         NULL,
    [OrderStatusId]         CHAR (2)      NOT NULL
);





