CREATE TABLE [dbo].[OrderComments] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Text]        VARCHAR (MAX) NOT NULL,
    [DateCreated] DATETIME2 (7) CONSTRAINT [DF_OrderComments_DateCreated] DEFAULT (getdate()) NOT NULL,
    [UserId]      VARCHAR (10)  NOT NULL,
    [OrderId]     INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_OrderComments_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT [FK_OrderComments_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [OrderComments_UserId_IDX]
    ON [dbo].[OrderComments]([UserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [OrderComments_OrderId_IDX]
    ON [dbo].[OrderComments]([OrderId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



