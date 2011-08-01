ALTER TABLE [dbo].[WorkgroupAddresses]
    ADD CONSTRAINT [FK_WorkgroupAddresses_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

