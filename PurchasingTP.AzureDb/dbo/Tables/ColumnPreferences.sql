CREATE TABLE [dbo].[ColumnPreferences] (
    [Id]                      VARCHAR (10) NOT NULL,
    [ShowRequestNumber]       BIT          CONSTRAINT [DF__ColumnPre__ShowR__16CE6296] DEFAULT ((1)) NOT NULL,
    [ShowRequestType]         BIT          CONSTRAINT [DF_ColumnPreferences_ShowRequestType] DEFAULT ((0)) NOT NULL,
    [ShowWorkgroup]           BIT          CONSTRAINT [DF__ColumnPre__ShowW__17C286CF] DEFAULT ((1)) NOT NULL,
    [ShowVendor]              BIT          CONSTRAINT [DF__ColumnPre__ShowV__18B6AB08] DEFAULT ((1)) NOT NULL,
    [ShowCreatedBy]           BIT          CONSTRAINT [DF__ColumnPre__ShowC__0D44F85C] DEFAULT ((1)) NOT NULL,
    [ShowCreatedDate]         BIT          CONSTRAINT [DF__ColumnPre__ShowC__0E391C95] DEFAULT ((1)) NOT NULL,
    [ShowLineItems]           BIT          CONSTRAINT [DF__ColumnPre__ShowL__0F2D40CE] DEFAULT ((1)) NOT NULL,
    [ShowTotalAmount]         BIT          CONSTRAINT [DF__ColumnPre__ShowT__10216507] DEFAULT ((1)) NOT NULL,
    [ShowStatus]              BIT          CONSTRAINT [DF__ColumnPre__ShowS__1209AD79] DEFAULT ((1)) NOT NULL,
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
    [ShowTag]                 BIT          CONSTRAINT [DF_ColumnPreferences_ShowTag] DEFAULT ((0)) NOT NULL,
    [ShowLastActedOnDate]     BIT          NOT NULL,
    [ShowDaysNotActedOn]      BIT          NOT NULL,
    [ShowLastActedOnBy]       BIT          NOT NULL,
    [ShowOrderReceived]       BIT          CONSTRAINT [DF_ColumnPreferences_ShowOrderReceived] DEFAULT ((0)) NOT NULL,
    [ShowOrderType]           BIT          NOT NULL,
    [DisplayRows]             INT          CONSTRAINT [DF__ColumnPre__Displ__12FDD1B2] DEFAULT ((50)) NOT NULL,
    [ShowHasSplits]           BIT          CONSTRAINT [DF__ColumnPre__ShowH__13F1F5EB] DEFAULT ((0)) NOT NULL,
    [ShowShippingCost]        BIT          CONSTRAINT [DF__ColumnPre__ShowS__14E61A24] DEFAULT ((0)) NOT NULL,
    [ShowReferenceNumber]     BIT          CONSTRAINT [DF__ColumnPre__ShowR__15DA3E5D] DEFAULT ((1)) NOT NULL,
    [ShowFpdCompleted]        BIT          CONSTRAINT [DF__ColumnPre__ShowF__11158940] DEFAULT ((0)) NOT NULL,
    [ShowOrderPaid]           BIT          CONSTRAINT [DF__ColumnPre__ShowO__19AACF41] DEFAULT ((0)) NOT NULL,
    [ShowApUser] BIT NOT NULL DEFAULT ((0)), 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);







GO
CREATE NONCLUSTERED INDEX [ColumnPreferences_UserId_IDX]
    ON [dbo].[ColumnPreferences]([Id] ASC);

