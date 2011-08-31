CREATE TABLE [dbo].[EmailQueue] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [UserId]           VARCHAR (10)     NULL,
    [Text]             VARCHAR (MAX)    NOT NULL,
    [OrderId]          INT              NOT NULL,
    [DateTimeCreated]  DATETIME2 (7)    NOT NULL,
    [Pending]          BIT              NOT NULL,
    [DateTimeSent]     DATETIME2 (7)    NULL,
    [Status]           VARCHAR (MAX)    NULL,
    [Email]            VARCHAR (100)    NULL,
    [NotificationType] VARCHAR (50)     NOT NULL
);





