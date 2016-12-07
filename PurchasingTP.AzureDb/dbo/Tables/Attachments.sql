CREATE TABLE [dbo].[Attachments] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Attachments_Id] DEFAULT (newid()) NOT NULL,
    [Filename]    VARCHAR (250)    NOT NULL,
    [ContentType] VARCHAR (200)    NOT NULL,
    [Contents]    VARBINARY (MAX)  NULL,
    [OrderId]     INT              NULL,
    [DateCreated] DATETIME         CONSTRAINT [DF_Attachments_DateCreated] DEFAULT (getdate()) NOT NULL,
    [UserId]      VARCHAR (10)     NOT NULL,
    [Category]    VARCHAR (50)     NULL,
    [MessageId]   VARCHAR (250)    NULL,
    [IsBlob]      BIT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_Attachments_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT [FK_Attachments_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [HistoryReceivedLineItems_UserID_IDX]
    ON [dbo].[Attachments]([OrderId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Attachments_UserId_IDX]
    ON [dbo].[Attachments]([UserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);




GO
CREATE NONCLUSTERED INDEX [Attachments_OrderId_IDX]
    ON [dbo].[Attachments]([OrderId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



