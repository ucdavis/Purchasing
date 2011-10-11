CREATE TABLE [dbo].[Splits] (
    [Id]         INT          IDENTITY (1, 1) NOT NULL,
    [OrderId]    INT          NOT NULL,
    [LineItemId] INT          NULL,
    [Amount]     MONEY        NOT NULL,
    [Account]    VARCHAR (10) NULL,
    [SubAccount] VARCHAR (5)  NULL,
    [Project]    VARCHAR (10) NULL
);









