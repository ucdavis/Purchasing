CREATE TABLE [dbo].[Splits] (
    [Id]         INT          IDENTITY (1, 1) NOT NULL,
    [OrderId]    INT          NOT NULL,
    [LineItemId] INT          NULL,
    [Amount]     MONEY        NOT NULL,
    [Account]    VARCHAR (10) NULL,
    [SubAccount] VARCHAR (5)  NULL,
    [Project]    VARCHAR (10) NULL,
    CONSTRAINT [PK_Splits_1] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF),
    CONSTRAINT [FK_Splits_LineItems] FOREIGN KEY ([LineItemId]) REFERENCES [dbo].[LineItems] ([Id]),
    CONSTRAINT [FK_Splits_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);

