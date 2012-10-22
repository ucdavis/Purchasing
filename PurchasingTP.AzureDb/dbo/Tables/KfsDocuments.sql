CREATE TABLE [dbo].[KfsDocuments] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [DocNumber] VARCHAR (50) NOT NULL,
    [OrderId]   INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DocumentNumbers_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id])
);
