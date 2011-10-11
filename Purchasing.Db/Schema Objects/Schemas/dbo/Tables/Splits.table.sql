CREATE TABLE [dbo].[Splits] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [OrderId]      INT          NOT NULL,
    [LineItemId]   INT          NULL,
    [Amount]       MONEY        NOT NULL,
    [AccountId]    VARCHAR (10) NULL,
    [SubAccountId] VARCHAR (5)  NULL,
    [ProjectId]    VARCHAR (10) NULL
);







