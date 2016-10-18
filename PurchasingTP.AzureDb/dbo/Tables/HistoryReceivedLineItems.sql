CREATE TABLE [dbo].[HistoryReceivedLineItems] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [LineItemId]          INT             NOT NULL,
    [UpdateDate]          DATETIME        NOT NULL,
    [OldReceivedQuantity] DECIMAL (18, 3) NULL,
    [NewReceivedQuantity] DECIMAL (18, 3) NULL,
    [UserId]              VARCHAR (10)    NOT NULL,
    [CommentsUpdated]     BIT             CONSTRAINT [DF_HistoryReceivedLineItems_CommentsUpdated] DEFAULT ((0)) NOT NULL,
    [PayInvoice]          BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON)
);




GO
CREATE NONCLUSTERED INDEX [HistoryReceivedLineItems_UserID_IDX]
    ON [dbo].[HistoryReceivedLineItems]([UserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



