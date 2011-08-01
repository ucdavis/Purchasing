ALTER TABLE [dbo].[WorkgroupVendors]
    ADD CONSTRAINT [FK_WorkgroupVendors_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

