CREATE TABLE [dbo].[EmailQueue] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [UserId]           VARCHAR (10)     NULL,
    [Text]             VARCHAR (MAX)    NOT NULL,
    [OrderId]          INT              NOT NULL,
    [DateTimeCreated]  DATETIME2 (7)    CONSTRAINT [DF_EmailQueue_DateTimeCreated] DEFAULT (getdate()) NOT NULL,
    [Pending]          BIT              CONSTRAINT [DF_EmailQueue_Pending] DEFAULT ((1)) NOT NULL,
    [DateTimeSent]     DATETIME2 (7)    NULL,
    [Status]           VARCHAR (MAX)    NULL,
    [Email]            VARCHAR (100)    NULL,
    [NotificationType] VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_EmailQueue] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailQueue_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT [FK_EmailQueue_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

