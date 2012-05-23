CREATE TABLE [dbo].[ColumnPreferences] (
    [Id]                       VARCHAR (10) NOT NULL,
    [ShowRequestNumber]        BIT          DEFAULT ((1)) NOT NULL,
    [ShowWorkgroup]            BIT          DEFAULT ((1)) NOT NULL,
    [ShowVendor]               BIT          DEFAULT ((1)) NOT NULL,
    [ShowCreatedBy]            BIT          DEFAULT ((1)) NOT NULL,
    [ShowCreatedDate]          BIT          DEFAULT ((1)) NOT NULL,
    [ShowLineItems]            BIT          DEFAULT ((1)) NOT NULL,
    [ShowTotalAmount]          BIT          DEFAULT ((1)) NOT NULL,
    [ShowStatus]               BIT          DEFAULT ((1)) NOT NULL,
    [ShowApprover]             BIT          NOT NULL,
    [ShowAccountManager]       BIT          NOT NULL,
    [ShowPurchaser]            BIT          NOT NULL,
    [ShowAccountNumber]        BIT          NOT NULL,
    [ShowShipTo]               BIT          NOT NULL,
    [ShowAllowBackorder]       BIT          NOT NULL,
    [ShowRestrictedOrder]      BIT          NOT NULL,
    [ShowNeededDate]           BIT          NOT NULL,
    [ShowShippingType]         BIT          NOT NULL,
    [ShowPurchaseOrderNumber]  BIT          NOT NULL,
    [ShowLastActedOnDate]      BIT          NOT NULL,
    [ShowDaysNotActedOn]       BIT          NOT NULL,
    [ShowLastActedOnBy]        BIT          NOT NULL,
    [ShowOrderReceived]        BIT          NOT NULL,
    [ShowOrderType]            BIT          NOT NULL, 
    [DisplayRows] INT NOT NULL DEFAULT ((50))
);









