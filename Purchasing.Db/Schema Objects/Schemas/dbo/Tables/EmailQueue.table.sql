CREATE TABLE [dbo].[EmailQueue] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [UserId]           VARCHAR (10)     NOT NULL,
    [Text]             VARCHAR (MAX)    NOT NULL,
    [OrderId]          INT              NOT NULL,
    [DateTimeCreated]  DATETIME         NOT NULL,
    [Pending]          BIT              NOT NULL,
    [DateTimeSent]     DATETIME         NULL,
    [Status]           VARCHAR (MAX)    NULL,
    [Email]            VARCHAR (100)    NULL,
    [NotificationType] VARCHAR (50)     NOT NULL
);

