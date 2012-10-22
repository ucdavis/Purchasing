CREATE TABLE [dbo].[Notifications] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [UserId]   VARCHAR (10)     NOT NULL,
    [Created]  DATETIME         NOT NULL,
    [Sent]     DATETIME         NULL,
    [Status]   VARCHAR (MAX)    NULL,
    [Pending]  BIT              NOT NULL,
    [PerEvent] BIT              NOT NULL,
    [Daily]    BIT              NOT NULL,
    [Weekly]   BIT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [Notifications_UserId_IDX]
    ON [dbo].[Notifications]([UserId] ASC);

