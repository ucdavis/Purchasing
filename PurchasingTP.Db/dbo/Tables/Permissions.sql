CREATE TABLE [dbo].[Permissions] (
    [RoleId] CHAR (2)     NOT NULL,
    [UserId] VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([RoleId] ASC, [UserId] ASC)
);

