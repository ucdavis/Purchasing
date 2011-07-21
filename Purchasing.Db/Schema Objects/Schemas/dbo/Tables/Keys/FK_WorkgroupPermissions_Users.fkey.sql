ALTER TABLE [dbo].[WorkgroupPermissions]
    ADD CONSTRAINT [FK_WorkgroupPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

