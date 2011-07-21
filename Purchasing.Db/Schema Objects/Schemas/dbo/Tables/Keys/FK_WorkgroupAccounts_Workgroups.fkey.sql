ALTER TABLE [dbo].[WorkgroupAccounts]
    ADD CONSTRAINT [FK_WorkgroupAccounts_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

