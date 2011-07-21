ALTER TABLE [dbo].[WorkgroupPermissions]
    ADD CONSTRAINT [FK_WorkgroupPermissions_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

