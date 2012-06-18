CREATE TABLE [dbo].[Users] (
    [Id]        VARCHAR (10) NOT NULL,
    [FirstName] VARCHAR (50) NOT NULL,
    [LastName]  VARCHAR (50) NOT NULL,
    [Email]     VARCHAR (50) NOT NULL,
    [AwayUntil] DATETIME     NULL,
    [IsActive]  BIT          CONSTRAINT [DF_Users_IsActive] DEFAULT ((1)) NOT NULL,
    [IsAway]    AS           (isnull(CONVERT([bit],datediff(day,[awayuntil],getdate()),(0)),(0))),
    CONSTRAINT [PK_Users_1] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF)
);

