ALTER TABLE [dbo].[OrderTracking]
    ADD CONSTRAINT [FK_OrderTracking_OrderStatusCodes] FOREIGN KEY ([OrderStatusCodeId]) REFERENCES [dbo].[OrderStatusCodes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

