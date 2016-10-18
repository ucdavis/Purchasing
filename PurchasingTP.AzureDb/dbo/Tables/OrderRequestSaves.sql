CREATE TABLE [dbo].[OrderRequestSaves] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_OrderRequestSaves_Id] DEFAULT (newid()) NOT NULL,
    [Name]         VARCHAR (150)    NOT NULL,
    [UserId]       VARCHAR (10)     NOT NULL,
    [PreparedById] VARCHAR (10)     NULL,
    [WorkgroupId]  INT              NOT NULL,
    [FormData]     VARCHAR (MAX)    NOT NULL,
    [AccountData]  VARCHAR (MAX)    NOT NULL,
    [DateCreated]  DATETIME         CONSTRAINT [DF_OrderRequestSaves_DateCreated] DEFAULT (getdate()) NOT NULL,
    [LastUpdate]   DATETIME         CONSTRAINT [DF_OrderRequestSaves_LastUpdate] DEFAULT (getdate()) NOT NULL,
    [Version]      VARCHAR (15)     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_OrderRequestSaves_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_OrderRequestSaves_Users1] FOREIGN KEY ([PreparedById]) REFERENCES [dbo].[Users] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [OrderRequestSaves_PreparedById_IDX]
    ON [dbo].[OrderRequestSaves]([PreparedById] ASC);


GO
CREATE NONCLUSTERED INDEX [OrderRequestSaves_UserId_IDX]
    ON [dbo].[OrderRequestSaves]([UserId] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



