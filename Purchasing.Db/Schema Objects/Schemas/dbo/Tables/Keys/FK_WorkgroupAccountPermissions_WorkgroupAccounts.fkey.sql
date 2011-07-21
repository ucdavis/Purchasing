ALTER TABLE [dbo].[WorkgroupAccountPermissions]
    ADD CONSTRAINT [FK_WorkgroupAccountPermissions_WorkgroupAccounts] FOREIGN KEY ([WorkgroupAccountId]) REFERENCES [dbo].[WorkgroupAccounts] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

