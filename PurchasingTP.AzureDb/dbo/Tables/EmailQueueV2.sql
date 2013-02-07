CREATE TABLE [dbo].[EmailQueueV2] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [UserId]           VARCHAR (10)     NULL,
	[Email]            VARCHAR (100)    NULL,
    [OrderId]          INT              NOT NULL,    
    [Pending]          BIT              CONSTRAINT [DF_EmailQueueV2_Pending] DEFAULT ((1)) NOT NULL,
    [DateTimeSent]     DATETIME2 (7)    NULL,
    [Status]           VARCHAR (MAX)    NULL,    
    [NotificationType] VARCHAR (50)     NOT NULL,
	[DateTimeCreated]  DATETIME2 (7)    CONSTRAINT [DF_EmailQueueV2_DateTimeCreated] DEFAULT (getdate()) NOT NULL,
    [Action] VARCHAR(MAX) NOT NULL, 
    [Details] VARCHAR(MAX) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailQueueV2_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT [FK_EmailQueueV2_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]), 
);


GO
CREATE NONCLUSTERED INDEX [EmailQueueV2_UserId_IDX]
    ON [dbo].[EmailQueue]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [EmailQueueV2_OrderId_IDX]
    ON [dbo].[EmailQueue]([OrderId] ASC);


