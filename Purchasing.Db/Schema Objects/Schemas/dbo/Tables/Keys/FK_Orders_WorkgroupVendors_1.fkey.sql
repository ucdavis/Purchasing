ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_WorkgroupVendors] FOREIGN KEY ([WorkgroupVendorId]) REFERENCES [dbo].[WorkgroupVendors] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

