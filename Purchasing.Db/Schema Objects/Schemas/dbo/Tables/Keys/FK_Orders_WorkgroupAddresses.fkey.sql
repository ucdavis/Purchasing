ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_WorkgroupAddresses] FOREIGN KEY ([WorkgroupAddressId]) REFERENCES [dbo].[WorkgroupAddresses] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

