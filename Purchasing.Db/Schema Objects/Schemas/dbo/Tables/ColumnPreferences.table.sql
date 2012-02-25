CREATE TABLE [dbo].[ColumnPreferences] (
    
	[Id]                      VARCHAR (10) NOT NULL,

	/* default columns */
	[ShowRequestNumber]       BIT          NOT NULL		default		1,
	[ShowWorkgroup]           BIT          NOT NULL		default		1,
	[ShowVendor]              BIT          NOT NULL		default		1,
	[ShowCreatedBy]           BIT          NOT NULL		default		1,
	[ShowCreatedDate]         BIT          NOT NULL		default		1,
	[ShowLineItems]			  BIT		   NOT NULL		default		1,
	[ShowTotalAmount]         BIT          NOT NULL		default		1,
	[ShowStatus]              BIT          NOT NULL		default		1,

	/* optional columns */
    [ShowApprover]            BIT          NOT NULL,
    [ShowAccountManager]      BIT          NOT NULL,
    [ShowPurchaser]           BIT          NOT NULL,
	[ShowAccountNumber]       BIT          NOT NULL,
	[ShowShipTo]              BIT          NOT NULL,
	[ShowAllowBackorder]      BIT          NOT NULL,
	[ShowRestrictedOrder]     BIT          NOT NULL,
	[ShowNeededDate]          BIT          NOT NULL,
	[ShowShippingType]        BIT          NOT NULL,
	[ShowPurchaseOrderNumber] BIT          NOT NULL,
    [ShowLastActedOnDate]     BIT          NOT NULL,
	[ShowDaysNotActedOn]      BIT          NOT NULL,
	[ShowLastActedOnBy]       BIT          NOT NULL,
    
	/*
	Not needed anymore
	[ShowOrganization]        BIT          NOT NULL,
    [ShowHasSplits]           BIT          NOT NULL,
    [ShowHasAttachments]      BIT          NOT NULL,
    [ShowNumberOfLines]       BIT          NOT NULL,
    [ShowPeoplePendingAction] BIT          NOT NULL,
    [ShowOrderedDate]         BIT          NOT NULL,
    [ShowLastYouActedOnDate]  BIT          NOT NULL
	*/
);



