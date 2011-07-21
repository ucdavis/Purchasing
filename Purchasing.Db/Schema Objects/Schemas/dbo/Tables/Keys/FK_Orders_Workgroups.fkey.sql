ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_Workgroups] FOREIGN KEY ([WorkgroupId]) REFERENCES [dbo].[Workgroups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

