CREATE TABLE [dbo].[OrderStatusCodes] (
    [Id]               CHAR (2)     NOT NULL,
    [Name]             VARCHAR (50) NOT NULL,
    [Level]            INT          NULL,
    [IsComplete]       BIT          CONSTRAINT [DF_OrderStatusCodes_IsComplete] DEFAULT ((0)) NOT NULL,
    [KfsStatus]        BIT          CONSTRAINT [DF_OrderStatusCodes_KfsStatus] DEFAULT ((0)) NOT NULL,
    [ShowInFilterList] BIT          CONSTRAINT [DF_OrderStatusCodes_ShowInFilterList] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_OrderStatusCodes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [OrderStatusCodes_Level_IDX]
    ON [dbo].[OrderStatusCodes]([Level] ASC);


GO
CREATE NONCLUSTERED INDEX [OrderStatusCodes_IsComplete_IDX]
    ON [dbo].[OrderStatusCodes]([IsComplete] ASC);

