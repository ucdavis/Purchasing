CREATE TABLE [dbo].[HistoryReceivedLineItems] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [LineItemId]          INT             NOT NULL,
    [UpdateDate]          DATETIME        NOT NULL,
    [OldReceivedQuantity] DECIMAL (18, 3) NULL,
    [NewReceivedQuantity] DECIMAL (18, 3) NULL,
    [UserId]              VARCHAR (10)    NOT NULL
);

