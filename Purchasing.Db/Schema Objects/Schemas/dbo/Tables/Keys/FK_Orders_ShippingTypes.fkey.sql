ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_ShippingTypes] FOREIGN KEY ([ShippingTypeId]) REFERENCES [dbo].[ShippingTypes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

