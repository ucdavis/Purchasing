CREATE TABLE [dbo].[Permissions] (
    [RoleId] CHAR (2)     NOT NULL,
    [UserId] VARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([RoleId] ASC, [UserId] ASC)
);



GO
CREATE NONCLUSTERED INDEX [Permissions_UserId_IDX]
    ON [dbo].[Permissions]([UserId] ASC);

