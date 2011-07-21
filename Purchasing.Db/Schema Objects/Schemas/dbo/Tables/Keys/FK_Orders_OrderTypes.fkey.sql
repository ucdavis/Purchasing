ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_OrderTypes] FOREIGN KEY ([OrderTypeId]) REFERENCES [dbo].[OrderTypes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

