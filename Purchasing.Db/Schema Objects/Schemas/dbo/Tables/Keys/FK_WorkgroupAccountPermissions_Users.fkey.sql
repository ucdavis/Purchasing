ALTER TABLE [dbo].[WorkgroupAccountPermissions]
    ADD CONSTRAINT [FK_WorkgroupAccountPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

