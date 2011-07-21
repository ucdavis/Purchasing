ALTER TABLE [dbo].[OrderTracking]
    ADD CONSTRAINT [FK_OrderTracking_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

