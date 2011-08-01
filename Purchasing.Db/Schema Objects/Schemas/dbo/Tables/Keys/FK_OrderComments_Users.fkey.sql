ALTER TABLE [dbo].[OrderComments]
    ADD CONSTRAINT [FK_OrderComments_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

