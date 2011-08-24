ALTER TABLE [dbo].[WorkgroupPermissions]
    ADD CONSTRAINT [FK_WorkgroupPermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;



