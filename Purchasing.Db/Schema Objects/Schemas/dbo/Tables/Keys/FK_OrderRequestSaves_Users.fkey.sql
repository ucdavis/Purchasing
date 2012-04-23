ALTER TABLE [dbo].[OrderRequestSaves]
    ADD CONSTRAINT [FK_OrderRequestSaves_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

