ALTER TABLE [dbo].[WorkgroupAccountPermissions]
    ADD CONSTRAINT [FK_WorkgroupAccountPermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

