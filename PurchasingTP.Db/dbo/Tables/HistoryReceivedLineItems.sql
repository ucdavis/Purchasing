CREATE TABLE [dbo].[HistoryReceivedLineItems] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [LineItemId]          INT             NOT NULL,
    [UpdateDate]          DATETIME        NOT NULL,
    [OldReceivedQuantity] DECIMAL (18, 3) NULL,
    [NewReceivedQuantity] DECIMAL (18, 3) NULL,
    [UserId]              VARCHAR (10)    NOT NULL,
    [CommentsUpdated]     BIT             CONSTRAINT [DF_HistoryReceivedLineItems_CommentsUpdated] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_HistoryReceivedLineItems] PRIMARY KEY CLUSTERED ([Id] ASC)
);

