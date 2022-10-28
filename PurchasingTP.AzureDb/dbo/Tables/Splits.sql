CREATE TABLE [dbo].[Splits] (
    [Id]         INT          IDENTITY (1, 1) NOT NULL,
    [OrderId]    INT          NOT NULL,
    [LineItemId] INT          NULL,
    [Amount]     MONEY        NOT NULL,
    [Account]    VARCHAR (10) NULL,
    [SubAccount] VARCHAR (5)  NULL,
    [Project]    VARCHAR (10) NULL,
    [Name] VARCHAR(64) NULL, 
    [FinancialSegmentString] VARBINARY(128) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_Splits_LineItems] FOREIGN KEY ([LineItemId]) REFERENCES [dbo].[LineItems] ([Id]),
    CONSTRAINT [FK_Splits_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);





GO
CREATE NONCLUSTERED INDEX [Splits_OrderId_IDX]
    ON [dbo].[Splits]([OrderId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Splits_LineItemId_IDX]
    ON [dbo].[Splits]([LineItemId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



