CREATE TABLE [dbo].[Roles] (
    [Id]      CHAR (2)     NOT NULL,
    [Name]    VARCHAR (50) NOT NULL,
    [Level]   INT          NOT NULL,
    [IsAdmin] BIT          DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_RoleTypes_1] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF)
);

