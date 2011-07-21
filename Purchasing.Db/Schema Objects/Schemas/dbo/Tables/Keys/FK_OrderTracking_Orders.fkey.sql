ALTER TABLE [dbo].[OrderTracking]
    ADD CONSTRAINT [FK_OrderTracking_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

