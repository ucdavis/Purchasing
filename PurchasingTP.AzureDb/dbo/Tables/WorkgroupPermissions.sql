CREATE TABLE [dbo].[WorkgroupPermissions] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [WorkgroupId]       INT          NOT NULL,
    [UserId]            VARCHAR (10) NOT NULL,
    [RoleId]            CHAR (2)     NOT NULL,
    [IsAdmin]           BIT          DEFAULT ((0)) NOT NULL,
    [ParentWorkgroupId] INT          NULL,
    [IsFullFeatured]    BIT          DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkgroupPermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
    CONSTRAINT [FK_WorkgroupPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_WorkgroupPermissions_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id])
);



GO
CREATE NONCLUSTERED INDEX [WorkgroupPermissions_workgroupid]
    ON [dbo].[WorkgroupPermissions]([WorkgroupId] ASC);


GO
CREATE NONCLUSTERED INDEX [WorkgroupPermissions_UserId_IDX]
    ON [dbo].[WorkgroupPermissions]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [WorkgroupPermissions_roleid_IDX]
    ON [dbo].[WorkgroupPermissions]([RoleId] ASC);

