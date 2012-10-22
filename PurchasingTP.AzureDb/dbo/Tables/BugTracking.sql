CREATE TABLE [dbo].[BugTracking] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [OrderId]         INT           NOT NULL,
    [UserId]          VARCHAR (20)  NOT NULL,
    [DateTimeStamp]   DATETIME      NOT NULL,
    [TrackingMessage] VARCHAR (500) NULL,
    [SplitId]         INT           NULL,
    [LineItemId]      INT           NULL,
    CONSTRAINT [PK_BugTracking] PRIMARY KEY CLUSTERED ([Id] ASC)
);

