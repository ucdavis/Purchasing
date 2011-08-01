ALTER TABLE [dbo].[OrderComments]
    ADD CONSTRAINT [FK_OrderComments_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

