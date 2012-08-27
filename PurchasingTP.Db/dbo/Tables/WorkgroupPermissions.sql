CREATE TABLE [dbo].[WorkgroupPermissions] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [WorkgroupId] INT          NOT NULL,
    [UserId]      VARCHAR (10) NOT NULL,
    [RoleId]      CHAR (2)     NOT NULL,
    [IsAdmin] BIT NOT NULL DEFAULT ((0)), 
    [ParentWorkgroupId] INT NULL, 
    [IsFullFeatured] BIT NOT NULL DEFAULT ((0)), 
    CONSTRAINT [PK_WorkgroupUsers_1] PRIMARY KEY NONCLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = OFF, ALLOW_ROW_LOCKS = OFF),
    CONSTRAINT [FK_WorkgroupPermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [FK_WorkgroupPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_WorkgroupPermissions_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [WorkgroupPermissions_WorkgroupId]
    ON [dbo].[WorkgroupPermissions]([WorkgroupId] ASC);


GO
CREATE NONCLUSTERED INDEX [WorkgroupPermissions_UserId_IDX]
    ON [dbo].[WorkgroupPermissions]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [WorkgroupPermissions_RoleId_IDX]
    ON [dbo].[WorkgroupPermissions]([RoleId] ASC);

