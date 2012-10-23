CREATE TABLE [dbo].[Users] (
    [Id]        VARCHAR (10) NOT NULL,
    [FirstName] VARCHAR (50) NOT NULL,
    [LastName]  VARCHAR (50) NOT NULL,
    [Email]     VARCHAR (50) NOT NULL,
    [AwayUntil] DATETIME     NULL,
    [IsActive]  BIT          CONSTRAINT [DF_Users_IsActive] DEFAULT ((1)) NOT NULL,
    [IsAway]    AS           (case when datediff(day,[AwayUntil],getdate())<(0) then (1) else (0) end),
    PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [Users_IdIsAway_CVDX]
    ON [dbo].[Users]([Id] ASC)
    INCLUDE([IsActive]);

